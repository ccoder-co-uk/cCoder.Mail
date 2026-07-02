using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class SmtpMailSenderProvider(IMailClient mailClient) : IMailSenderProvider
{
    public string ProviderName => MailProviderNames.Smtp;

    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailClient.SendAsync(email, cancellationToken);
}
