using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

public interface IMailReceiverProvider
{
    string ProviderName { get; }

    Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default);
}
