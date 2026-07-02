using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Brokers.MailClients;

public interface ISmtpMailSenderBroker
{
    Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}
