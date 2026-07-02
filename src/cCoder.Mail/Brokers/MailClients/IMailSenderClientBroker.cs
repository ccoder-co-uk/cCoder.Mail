using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Brokers.MailClients;

public interface IMailSenderClientBroker
{
    Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}
