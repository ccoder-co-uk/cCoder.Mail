using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed class Pop3MailReceiverService(IPop3MailReceiverBroker pop3MailReceiverBroker)
    : IPop3MailReceiverService
{
    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        pop3MailReceiverBroker.ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        pop3MailReceiverBroker.ReceiveTopAsync(count, cancellationToken);
}
