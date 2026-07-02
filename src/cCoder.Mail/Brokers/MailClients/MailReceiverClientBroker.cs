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
            .GetReceiver(request?.ProviderName)
            .ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        mailReceiverFactory
            .GetReceiver(null)
            .ReceiveTopAsync(count, cancellationToken);
}
