using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Brokers.MailClients;

public interface IMailSenderProvider
{
    string ProviderName { get; }

    Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}
