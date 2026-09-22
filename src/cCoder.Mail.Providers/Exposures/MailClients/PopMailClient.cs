// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Providers.Services.Foundations;

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class PopMailClient(
    Services.Orchestrations.IPop3MailReceiverOrchestrationService pop3MailReceiverOrchestrationService)
    : IMailClient
{
    public string[] GetProviderNames() =>
        [MailProviderNames.Pop3];

    public MailClientOperation[] GetSupportedOperations() =>
        [MailClientOperation.Receive];

    public Task SendAsync(
        QueuedEmail queuedEmail,
        CancellationToken cancellationToken = default) =>
        throw new UnsupportedMailClientOperationException(
            providerName: MailProviderNames.Pop3,
            operation: "Send");

    public Task<ReceivedEmail[]> ReceiveAsync(
        Guid mailReceiverId,
        int maximumMessages,
        CancellationToken cancellationToken = default) =>
        pop3MailReceiverOrchestrationService.ReceiveMailReceiverAsync(
            mailReceiverId: mailReceiverId,
            maximumMessages: maximumMessages,
            cancellationToken: cancellationToken);
}