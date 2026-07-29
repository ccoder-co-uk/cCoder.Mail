// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailReceiverClientBroker(IMailClientFactory mailClientFactory)
    : IMailReceiverClientBroker
{
    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        ReceiveMailReceiverAsync(
            request: request,
            cancellationToken: cancellationToken);

    private async Task<ReceivedEmail[]> ReceiveMailReceiverAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken)
    {
        Guid mailReceiverId =
            request?.MailReceiverId
            ?? throw new InvalidOperationException(
                message:
                    "A mail receiver ID is required.");

        IMailClient mailClient =
            await mailClientFactory.CreateMailClientAsync(
                mailReceiverId: mailReceiverId,
                cancellationToken: cancellationToken);

        return await mailClient.ReceiveAsync(
            mailReceiverId: mailReceiverId,
            maximumMessages: request.MaximumMessages,
            cancellationToken: cancellationToken);
    }

    public async Task<ReceivedEmail[]> ReceiveTopAsync(
        Guid mailReceiverId,
        int count,
        CancellationToken cancellationToken = default)
    {
        IMailClient mailClient =
            await mailClientFactory.CreateMailClientAsync(
                mailReceiverId: mailReceiverId,
                cancellationToken: cancellationToken);

        return await mailClient.ReceiveAsync(
            mailReceiverId: mailReceiverId,
            maximumMessages: count,
            cancellationToken: cancellationToken);
    }
}