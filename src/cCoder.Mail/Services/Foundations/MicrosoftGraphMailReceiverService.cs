// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http.Headers;
using System.Text.Json;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MicrosoftGraphMailReceiverService(
    MailConfiguration mailConfiguration,
    IMicrosoftGraphBroker microsoftGraphBroker)
    : IMicrosoftGraphMailReceiverService
{
    private const string DefaultGraphBaseUrl = "https://graph.microsoft.com/v1.0";

    private const string DefaultLoginBaseUrl = "https://login.microsoftonline.com";

    public Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: async () =>
        {
            ValidateReceiveAsync(inputs: [request, cancellationToken]);

            ValidateReceiveRequest(request: request);
            string accessToken = await GetAccessTokenAsync(cancellationToken: cancellationToken);
            using HttpRequestMessage message = new(method: HttpMethod.Get, requestUri: BuildMessagesUrl(request: request));
            message.Headers.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: accessToken);

            HttpClientBrokerResponse response = await microsoftGraphBroker.SendAsync(request: message, cancellationToken: cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(message: $"Microsoft Graph mailbox receive failed: {response.Content}");
            }

            return ParseMessages(content: response.Content);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {

            ValidateReceiveTopAsync(inputs: [count, cancellationToken]);

            return ReceiveMailboxReceiveRequestAsync(
            request: new MailboxReceiveRequest
            {
                User = ReadConfiguredReceiveUser(),
                MaximumMessages = count,
            },
            cancellationToken: cancellationToken);
        }, isTask: true);

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Post, requestUri: BuildTokenUrl())
        {
            Content = new FormUrlEncodedContent(
nameValueCollection: [
                new KeyValuePair<string, string>(
key: "client_id",
value: ReadRequiredConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.ClientId, configurationName: "Microsoft Graph client id")),
                new KeyValuePair<string, string>(
key: "client_secret",
value: ReadRequiredConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.ClientSecret, configurationName: "Microsoft Graph client secret")),
                new KeyValuePair<string, string>(key: "scope", value: "https://graph.microsoft.com/.default"),
                new KeyValuePair<string, string>(key: "grant_type", value: "client_credentials"),
            ]),
        };

        HttpClientBrokerResponse response = await microsoftGraphBroker.SendAsync(request: request, cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(message: $"Microsoft Graph token request failed: {response.Content}");
        }

        using JsonDocument document = JsonDocument.Parse(json: response.Content);

        return document.RootElement.GetProperty(propertyName: "access_token")
            .GetString()
            ?? throw new InvalidOperationException(message: "Microsoft Graph token response did not include an access token.");
    }

    private string BuildTokenUrl()
    {
        string tenantId = ReadRequiredConfiguredValue(
configuredValue: mailConfiguration.MicrosoftGraph.TenantId,
configurationName: "Microsoft Graph tenant id");

        string loginBaseUrl = ReadConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.LoginBaseUrl)
            ?? DefaultLoginBaseUrl;

        return $"{loginBaseUrl.TrimEnd(trimChar: '/')}/{Uri.EscapeDataString(stringToEscape: tenantId)}/oauth2/v2.0/token";
    }

    private string BuildMessagesUrl(MailboxReceiveRequest request)
    {
        string graphBaseUrl = ReadConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.GraphBaseUrl)
            ?? DefaultGraphBaseUrl;

        List<string> query =
        [
            "$select=internetMessageId,subject,body,receivedDateTime,from,toRecipients,ccRecipients",
            $"$top={Math.Clamp(value: request.MaximumMessages <= 0 ? 100 : request.MaximumMessages, min: 1, max: 100)}",
            "$orderby=receivedDateTime desc",
        ];

        string filter = BuildFilter(request: request);

        if (!string.IsNullOrWhiteSpace(value: filter))
        {
            query.Add(item: $"$filter={Uri.EscapeDataString(stringToEscape: filter)}");
        }

        return $"{graphBaseUrl.TrimEnd(trimChar: '/')}/users/{Uri.EscapeDataString(stringToEscape: request.User)}/mailFolders/inbox/messages"
            + $"?{string.Join(separator: "&", values: query)}";
    }

    private static string BuildFilter(MailboxReceiveRequest request)
    {
        List<string> filters = [];

        if (request.From is not null)
        {
            filters.Add(item: $"receivedDateTime ge {request.From.Value.UtcDateTime:O}");
        }

        if (request.To is not null)
        {
            filters.Add(item: $"receivedDateTime le {request.To.Value.UtcDateTime:O}");
        }

        return string.Join(separator: " and ", values: filters);
    }

    private static ReceivedEmail[] ParseMessages(string content)
    {
        using JsonDocument document = JsonDocument.Parse(json: content);

        if (!document.RootElement.TryGetProperty(propertyName: "value", value: out JsonElement messages))
        {
            return [];
        }

        return messages.EnumerateArray()
            .Select(selector: ParseMessage)
            .ToArray();
    }

    private static ReceivedEmail ParseMessage(JsonElement message) =>
        new()
        {
            MessageId = GetString(element: message, propertyName: "internetMessageId"),
            From = GetEmailAddress(message: message, propertyName: "from"),
            To = GetRecipientAddresses(message: message, propertyName: "toRecipients"),
            CC = GetRecipientAddresses(message: message, propertyName: "ccRecipients"),
            Subject = GetString(element: message, propertyName: "subject"),
            Content = GetBodyContent(message: message),
            IsBodyHtml = IsHtmlBody(message: message),
            ReceivedOn = GetReceivedOn(message: message),
        };

    private static string GetBodyContent(JsonElement message) =>
        message.TryGetProperty(propertyName: "body", value: out JsonElement body)
            ? GetString(element: body, propertyName: "content")
            : null;

    private static bool IsHtmlBody(JsonElement message) =>
        message.TryGetProperty(propertyName: "body", value: out JsonElement body)
        && string.Equals(a: GetString(element: body, propertyName: "contentType"), b: "html", comparisonType: StringComparison.OrdinalIgnoreCase);

    private static DateTimeOffset GetReceivedOn(JsonElement message) =>
        DateTimeOffset.TryParse(input: GetString(element: message, propertyName: "receivedDateTime"), result: out DateTimeOffset receivedOn)
            ? receivedOn
            : DateTimeOffset.MinValue;

    private static string GetEmailAddress(JsonElement message, string propertyName)
    {
        if (!message.TryGetProperty(propertyName: propertyName, value: out JsonElement recipient))
        {
            return null;
        }

        if (!recipient.TryGetProperty(propertyName: "emailAddress", value: out JsonElement emailAddress))
        {
            return null;
        }

        return GetString(element: emailAddress, propertyName: "address");
    }

    private static string GetRecipientAddresses(JsonElement message, string propertyName)
    {
        if (!message.TryGetProperty(propertyName: propertyName, value: out JsonElement recipients))
        {
            return null;
        }

        return string.Join(
separator: ", ",
values: recipients.EnumerateArray()
            .Select(selector: recipient => recipient.TryGetProperty(propertyName: "emailAddress", value: out JsonElement emailAddress)
                    ? GetString(element: emailAddress, propertyName: "address")
                    : null)
            .Where(predicate: address => !string.IsNullOrWhiteSpace(value: address)));
    }

    private static string GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName: propertyName, value: out JsonElement property)
        && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private string ReadConfiguredReceiveUser() =>
        ReadConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.ReceiveUser)
        ?? throw new InvalidOperationException(
message: "CCODER_MAIL_RECEIVE_USER is required for Microsoft Graph mailbox receive.");

    private static string ReadRequiredConfiguredValue(string configuredValue, string configurationName) =>
        ReadConfiguredValue(configuredValue: configuredValue)
        ?? throw new InvalidOperationException(message: $"{configurationName} is required for Microsoft Graph mail.");

    private static string ReadConfiguredValue(string configuredValue) =>
        string.IsNullOrWhiteSpace(value: configuredValue) ? null : configuredValue;

    private static void ValidateReceiveRequest(MailboxReceiveRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(paramName: nameof(request));
        }

        if (string.IsNullOrWhiteSpace(value: request.User))
        {
            throw new InvalidOperationException(message: "Mailbox user is required.");
        }
    }
}