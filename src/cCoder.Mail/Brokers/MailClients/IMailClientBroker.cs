using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

public interface IMailClientBroker
{
    Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);
}
