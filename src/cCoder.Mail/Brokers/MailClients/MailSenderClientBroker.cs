using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailSenderClientBroker(IMailSenderFactory mailSenderFactory)
    : IMailSenderClientBroker
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailSenderFactory
            .GetSender(GetSenderProviderName(email))
            .SendAsync(email, cancellationToken);

    private static string GetSenderProviderName(QueuedEmail email)
    {
        MailSender sender = email?.App?.MailSenders?.FirstOrDefault(
            mailSender => mailSender.Name == email.MailServerName);

        return sender?.ProviderName;
    }
}
