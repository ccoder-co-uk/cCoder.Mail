using System.Net.Http.Headers;
using System.Text.Json;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed class MicrosoftGraphMailReceiverService(
    MailConfiguration mailConfiguration,
    IMicrosoftGraphBroker microsoftGraphBroker)
    : IMicrosoftGraphMailReceiverService
{
    private const string DefaultGraphBaseUrl = "https://graph.microsoft.com/v1.0";
    private const string DefaultLoginBaseUrl = "https://login.microsoftonline.com";

    public async Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateReceiveRequest(request);
        string accessToken = await GetAccessTokenAsync(cancellationToken);
        using HttpRequestMessage message = new(HttpMethod.Get, BuildMessagesUrl(request));
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpClientBrokerResponse response = await microsoftGraphBroker.SendAsync(message, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Microsoft Graph mailbox receive failed: {response.Content}");

        return ParseMessages(response.Content);
    }

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        ReceiveAsync(
            new MailboxReceiveRequest
            {
                User = ReadConfiguredReceiveUser(),
                MaximumMessages = count,
            },
            cancellationToken);

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, BuildTokenUrl())
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>(
                    "client_id",
                    ReadRequiredConfiguredValue(mailConfiguration.MicrosoftGraph.ClientId, "Microsoft Graph client id")),
                new KeyValuePair<string, string>(
                    "client_secret",
                    ReadRequiredConfiguredValue(mailConfiguration.MicrosoftGraph.ClientSecret, "Microsoft Graph client secret")),
                new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            ]),
        };

        HttpClientBrokerResponse response = await microsoftGraphBroker.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Microsoft Graph token request failed: {response.Content}");

        using JsonDocument document = JsonDocument.Parse(response.Content);

        return document.RootElement.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("Microsoft Graph token response did not include an access token.");
    }

    private string BuildTokenUrl()
    {
        string tenantId = ReadRequiredConfiguredValue(
            mailConfiguration.MicrosoftGraph.TenantId,
            "Microsoft Graph tenant id");
        string loginBaseUrl = ReadConfiguredValue(mailConfiguration.MicrosoftGraph.LoginBaseUrl)
            ?? DefaultLoginBaseUrl;

        return $"{loginBaseUrl.TrimEnd('/')}/{Uri.EscapeDataString(tenantId)}/oauth2/v2.0/token";
    }

    private string BuildMessagesUrl(MailboxReceiveRequest request)
    {
        string graphBaseUrl = ReadConfiguredValue(mailConfiguration.MicrosoftGraph.GraphBaseUrl)
            ?? DefaultGraphBaseUrl;
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

    private static DateTimeOffset GetReceivedOn(JsonElement message) =>
        DateTimeOffset.TryParse(GetString(message, "receivedDateTime"), out DateTimeOffset receivedOn)
            ? receivedOn
            : DateTimeOffset.MinValue;

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

    private string ReadConfiguredReceiveUser() =>
        ReadConfiguredValue(mailConfiguration.MicrosoftGraph.ReceiveUser)
        ?? throw new InvalidOperationException(
            "CCODER_MAIL_RECEIVE_USER is required for Microsoft Graph mailbox receive.");

    private static string ReadRequiredConfiguredValue(string configuredValue, string configurationName) =>
        ReadConfiguredValue(configuredValue)
        ?? throw new InvalidOperationException($"{configurationName} is required for Microsoft Graph mail.");

    private static string ReadConfiguredValue(string configuredValue) =>
        string.IsNullOrWhiteSpace(configuredValue) ? null : configuredValue;

    private static void ValidateReceiveRequest(MailboxReceiveRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.User))
            throw new InvalidOperationException("Mailbox user is required.");
    }
}
