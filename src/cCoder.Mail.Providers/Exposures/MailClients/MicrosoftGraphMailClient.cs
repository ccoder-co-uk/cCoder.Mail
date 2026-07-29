// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Services.Foundations;

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class MicrosoftGraphMailClient(
    IMicrosoftGraphMailSenderService senderService,
    IMicrosoftGraphMailReceiverService receiverService)
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
        QueuedEmail email,
        CancellationToken cancellationToken = default) =>
        senderService.SendQueuedEmailAsync(
            email: email,
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