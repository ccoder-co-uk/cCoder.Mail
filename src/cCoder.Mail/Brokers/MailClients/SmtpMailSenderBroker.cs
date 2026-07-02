using System.Net;
using System.Net.Mail;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class SmtpMailSenderBroker : ISmtpMailSenderBroker
{
    public async Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default)
    {
        MailSender sender = email.App?.MailSenders?.FirstOrDefault(
            mailSender => mailSender.Name == email.MailServerName);

        if (sender == null)
            throw new InvalidOperationException("No mail sender configuration could be found to send the email.");

        using SmtpClient client = new()
        {
            Host = sender.Host,
            Port = sender.Port,
            EnableSsl = sender.EnableSSL,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(sender.User, sender.Password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        using MailMessage message = new()
        {
            IsBodyHtml = email.IsBodyHtml,
            Subject = email.Subject,
            Body = email.Content
        };

        if (!string.IsNullOrWhiteSpace(sender.FromEmail))
            message.From = new MailAddress(sender.FromEmail);

        message.From ??= sender.User.Contains('@')
            ? new MailAddress(sender.User)
            : null;

        message.To.Add(email.To);
        await client.SendMailAsync(message, cancellationToken);
    }
}
