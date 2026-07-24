// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Exposures.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailReceiverClientBroker(IMailReceiverFactory mailReceiverFactory)
    : IMailReceiverClientBroker
{
    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailReceiverFactory
            .GetReceiver(providerName: request?.ProviderName)
        .ReceiveAsync(request: request, cancellationToken: cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        mailReceiverFactory
            .GetReceiver(providerName: null)
        .ReceiveTopAsync(count: count, cancellationToken: cancellationToken);
}