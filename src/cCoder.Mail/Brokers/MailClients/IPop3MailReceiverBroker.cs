using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

public interface IPop3MailReceiverBroker
{
    Task<string[][]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);

    Task<string[][]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default);
}
