using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailSenderClientBroker(IMailSenderFactory mailSenderFactory)
    : IMailSenderClientBroker
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailSenderFactory
            .GetSender(email?.MailSender?.ProviderName)
            .SendAsync(email, cancellationToken);
}
