// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Services.Foundations;

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class MicrosoftGraphMailClient(
    IMicrosoftGraphMailSenderService senderService,
    Services.Orchestrations.IMicrosoftGraphMailReceiverOrchestrationService receiverService)
    : IMailClient
{
    public string[] GetProviderNames() =>
        [
            MailProviderNames.MicrosoftGraph,
            "graph.microsoft.com",
            "https://graph.microsoft.com",
            "microsoft-graph",
        ];

    public MailClientOperation[] GetSupportedOperations() =>
        [
            MailClientOperation.Send,
            MailClientOperation.Receive,
        ];

    public Task SendAsync(
        QueuedEmail queuedEmail,
        CancellationToken cancellationToken = default) =>
        senderService.SendQueuedEmailAsync(
            queuedEmail: queuedEmail,
            cancellationToken: cancellationToken);

    public Task<ReceivedEmail[]> ReceiveAsync(
        Guid mailReceiverId,
        int maximumMessages,
        CancellationToken cancellationToken = default) =>
        receiverService.ReceiveMailReceiverAsync(
            mailReceiverId: mailReceiverId,
            maximumMessages: maximumMessages,
            cancellationToken: cancellationToken);
}