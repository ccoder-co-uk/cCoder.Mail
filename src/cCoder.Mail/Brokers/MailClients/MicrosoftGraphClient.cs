using System.Net.Http.Headers;
using System.Text.Json;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MicrosoftGraphClient : IMicrosoftGraphClient
{
    private const string TenantIdVariableName = "CCODER_MAIL_GRAPH_TENANT_ID";
    private const string ClientIdVariableName = "CCODER_MAIL_GRAPH_CLIENT_ID";
    private const string ClientSecretVariableName = "CCODER_MAIL_GRAPH_CLIENT_SECRET";
    private const string GraphBaseUrlVariableName = "CCODER_MAIL_GRAPH_BASE_URL";
    private const string LoginBaseUrlVariableName = "CCODER_MAIL_GRAPH_LOGIN_BASE_URL";
    private const string DefaultGraphBaseUrl = "https://graph.microsoft.com/v1.0";
    private const string DefaultLoginBaseUrl = "https://login.microsoftonline.com";
    private static readonly HttpClient HttpClient = new();

    public async Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateReceiveRequest(request);
        string accessToken = await GetAccessTokenAsync(cancellationToken);
        using HttpRequestMessage message = new(HttpMethod.Get, BuildMessagesUrl(request));
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using HttpResponseMessage response = await HttpClient.SendAsync(message, cancellationToken);
        string content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Microsoft Graph mailbox receive failed: {content}");

        return ParseMessages(content);
    }

    private static async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        string tenantId = ReadRequiredEnvironment(TenantIdVariableName);
        string clientId = ReadRequiredEnvironment(ClientIdVariableName);
        string clientSecret = ReadRequiredEnvironment(ClientSecretVariableName);
        string loginBaseUrl = ReadEnvironment(LoginBaseUrlVariableName) ?? DefaultLoginBaseUrl;
        string tokenUrl = $"{loginBaseUrl.TrimEnd('/')}/{Uri.EscapeDataString(tenantId)}/oauth2/v2.0/token";

        using FormUrlEncodedContent request = new(
        [
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
        ]);

        using HttpResponseMessage response = await HttpClient.PostAsync(tokenUrl, request, cancellationToken);
        string content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Microsoft Graph token request failed: {content}");

        using JsonDocument document = JsonDocument.Parse(content);

        return document.RootElement.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("Microsoft Graph token response did not include an access token.");
    }

    private static string BuildMessagesUrl(MailboxReceiveRequest request)
    {
        string graphBaseUrl = ReadEnvironment(GraphBaseUrlVariableName) ?? DefaultGraphBaseUrl;
        List<string> query =
        [
            "$select=internetMessageId,subject,body,receivedDateTime,from,toRecipients,ccRecipients",
            $"$top={Math.Clamp(request.MaximumMessages <= 0 ? 100 : request.MaximumMessages, 1, 100)}",
            "$orderby=receivedDateTime desc",
        ];
        string filter = BuildFilter(request);

        if (!string.IsNullOrWhiteSpace(filter))
            query.Add($"$filter={Uri.EscapeDataString(filter)}");

        return $"{graphBaseUrl.TrimEnd('/')}/users/{Uri.EscapeDataString(request.User)}/mailFolders/inbox/messages"
            + $"?{string.Join("&", query)}";
    }

    private static string BuildFilter(MailboxReceiveRequest request)
    {
        List<string> filters = [];

        if (request.From is not null)
            filters.Add($"receivedDateTime ge {request.From.Value.UtcDateTime:O}");

        if (request.To is not null)
            filters.Add($"receivedDateTime le {request.To.Value.UtcDateTime:O}");

        return string.Join(" and ", filters);
    }

    private static ReceivedEmail[] ParseMessages(string content)
    {
        using JsonDocument document = JsonDocument.Parse(content);

        if (!document.RootElement.TryGetProperty("value", out JsonElement messages))
            return [];

        return messages.EnumerateArray()
            .Select(ParseMessage)
            .ToArray();
    }

    private static ReceivedEmail ParseMessage(JsonElement message) =>
        new()
        {
            MessageId = GetString(message, "internetMessageId"),
            From = GetEmailAddress(message, "from"),
            To = GetRecipientAddresses(message, "toRecipients"),
            CC = GetRecipientAddresses(message, "ccRecipients"),
            Subject = GetString(message, "subject"),
            Content = GetBodyContent(message),
            IsBodyHtml = IsHtmlBody(message),
            ReceivedOn = GetReceivedOn(message),
        };

    private static string GetBodyContent(JsonElement message) =>
        message.TryGetProperty("body", out JsonElement body)
            ? GetString(body, "content")
            : null;

    private static bool IsHtmlBody(JsonElement message) =>
        message.TryGetProperty("body", out JsonElement body)
        && string.Equals(GetString(body, "contentType"), "html", StringComparison.OrdinalIgnoreCase);

    private static DateTimeOffset? GetReceivedOn(JsonElement message) =>
        DateTimeOffset.TryParse(GetString(message, "receivedDateTime"), out DateTimeOffset receivedOn)
            ? receivedOn
            : null;

    private static string GetEmailAddress(JsonElement message, string propertyName)
    {
        if (!message.TryGetProperty(propertyName, out JsonElement recipient))
            return null;

        if (!recipient.TryGetProperty("emailAddress", out JsonElement emailAddress))
            return null;

        return GetString(emailAddress, "address");
    }

    private static string GetRecipientAddresses(JsonElement message, string propertyName)
    {
        if (!message.TryGetProperty(propertyName, out JsonElement recipients))
            return null;

        return string.Join(
            ", ",
            recipients.EnumerateArray()
                .Select(recipient => recipient.TryGetProperty("emailAddress", out JsonElement emailAddress)
                    ? GetString(emailAddress, "address")
                    : null)
                .Where(address => !string.IsNullOrWhiteSpace(address)));
    }

    private static string GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out JsonElement property)
        && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static string ReadRequiredEnvironment(string variableName) =>
        ReadEnvironment(variableName)
        ?? throw new InvalidOperationException($"{variableName} is required for Microsoft Graph mailbox receive.");

    private static string ReadEnvironment(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variableName)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static void ValidateReceiveRequest(MailboxReceiveRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.User))
            throw new InvalidOperationException("Mailbox user is required.");
    }
}
