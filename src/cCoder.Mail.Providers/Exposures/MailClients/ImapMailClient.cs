// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Providers.Services.Foundations;

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class ImapMailClient(
    Services.Orchestrations.IImapMailReceiverOrchestrationService imapMailReceiverOrchestrationService)
    : IMailClient
{
    public string[] GetProviderNames() =>
        [MailProviderNames.Imap];

    public MailClientOperation[] GetSupportedOperations() =>
        [MailClientOperation.Receive];

    public Task SendAsync(
        QueuedEmail queuedEmail,
        CancellationToken cancellationToken = default) =>
        throw new UnsupportedMailClientOperationException(
            providerName: MailProviderNames.Imap,
            operation: "Send");

    public Task<ReceivedEmail[]> ReceiveAsync(
        Guid mailReceiverId,
        int maximumMessages,
        CancellationToken cancellationToken = default) =>
        imapMailReceiverOrchestrationService.ReceiveMailReceiverAsync(
            mailReceiverId: mailReceiverId,
            maximumMessages: maximumMessages,
            cancellationToken: cancellationToken);
}