// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MicrosoftGraphMailReceiverService(
    MicrosoftGraphProviderConfiguration configuration,
    IMailReceiverStorageBroker mailReceiverStorageBroker,
    IMicrosoftGraphBroker microsoftGraphBroker)
    : IMicrosoftGraphMailReceiverService
{
    public Task<ReceivedEmail[]> ReceiveMailReceiverAsync(
        Guid mailReceiverId,
        int maximumMessages,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: async () =>
        {
            ValidateReceiveMailReceiverAsync(
                inputs:
                    [
                        mailReceiverId,
                        maximumMessages,
                        cancellationToken
                    ]);

            MailReceiver mailReceiver =
                await mailReceiverStorageBroker
                    .SelectMailReceiverByIdAsync(
                        mailReceiverId: mailReceiverId,
                        cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException(
                    message:
                        $"Mail receiver '{mailReceiverId}' was not found.");

            return await ReceiveMailboxAsync(
                request: new MailboxReceiveRequest
                {
                    ProviderName = mailReceiver.ProviderName,
                    AppId = mailReceiver.AppId,
                    MailReceiverId = mailReceiver.Id,
                    User = mailReceiver.User,
                    MaximumMessages = maximumMessages,
                },
                cancellationToken: cancellationToken);
        }, isTask: true);

    private async Task<ReceivedEmail[]> ReceiveMailboxAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken)
    {
        ValidateReceiveRequest(request: request);

        HttpClientBrokerResponse response =
            await microsoftGraphBroker.ReceiveEmailAsync(
                request: request,
                configuration: configuration,
                cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                message: $"Microsoft Graph mailbox receive failed: {response.Content}");
        }

        return ParseMessages(content: response.Content);
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