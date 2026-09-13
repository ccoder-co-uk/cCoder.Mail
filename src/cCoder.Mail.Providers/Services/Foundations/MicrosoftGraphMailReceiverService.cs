// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MicrosoftGraphMailReceiverService(
    MailProviderConfiguration configuration,
    IMailReceiverStorageBroker mailReceiverStorageBroker,
    IMicrosoftGraphBroker microsoftGraphBroker,
    IMailMessageParsingBroker mailMessageParsingBroker)
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

        MicrosoftGraphMessageEnvelope envelope =
            mailMessageParsingBroker.DeserializeMicrosoftGraphMessages(
                content: response.Content);

        return (envelope?.Value ?? [])
            .Select(selector: ParseMessage)
            .ToArray();
    }

    private static ReceivedEmail ParseMessage(MicrosoftGraphMessage message) =>
        new()
        {
            MessageId = message.InternetMessageId,
            From = message.From?.EmailAddress?.Address,
            To = GetRecipientAddresses(recipients: message.ToRecipients),
            CC = GetRecipientAddresses(recipients: message.CcRecipients),
            Subject = message.Subject,
            Content = message.Body?.Content,
            IsBodyHtml = string.Equals(
                a: message.Body?.ContentType,
                b: "html",
                comparisonType: StringComparison.OrdinalIgnoreCase),
            ReceivedOn = DateTimeOffset.TryParse(
                input: message.ReceivedDateTime,
                result: out DateTimeOffset receivedOn)
                    ? receivedOn
                    : DateTimeOffset.MinValue,
        };

    private static string GetRecipientAddresses(
        MicrosoftGraphRecipient[] recipients) =>
        string.Join(
            separator: ", ",
            values: (recipients ?? [])
                .Select(selector: recipient =>
                    recipient.EmailAddress?.Address)
                .Where(predicate: address =>
                    !string.IsNullOrWhiteSpace(value: address)));

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