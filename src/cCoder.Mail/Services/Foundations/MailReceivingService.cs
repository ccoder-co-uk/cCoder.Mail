// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailReceivingService(IMailReceiverClientBroker mailReceiverClientBroker) : IMailReceivingService
{
    public Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveMailboxReceiveRequestAsync(inputs: [request, cancellationToken]);

            return mailReceiverClientBroker.ReceiveAsync(request: request, cancellationToken: cancellationToken);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        Guid mailReceiverId,
        int count,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveTopAsync(
                inputs:
                    [
                        mailReceiverId,
                        count,
                        cancellationToken
                    ]);

            return mailReceiverClientBroker.ReceiveTopAsync(
                mailReceiverId: mailReceiverId,
                count: count,
                cancellationToken: cancellationToken);
        }, isTask: true);
}