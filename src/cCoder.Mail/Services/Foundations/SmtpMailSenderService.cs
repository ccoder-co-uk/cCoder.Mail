using System.Net.Mail;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed class SmtpMailSenderService(ISmtpMailSenderBroker smtpMailSenderBroker)
    : ISmtpMailSenderService
{
    public async Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default)
    {
        MailSender sender = email.MailSender
            ?? throw new InvalidOperationException("No mail sender configuration could be found to send the email.");

        using MailMessage message = CreateMailMessage(email, sender);

        await smtpMailSenderBroker.SendAsync(
            new SmtpMailSendRequest
            {
                Host = sender.Host,
                Port = sender.Port,
                EnableSsl = sender.EnableSSL,
                User = sender.User,
                Password = sender.Password,
                Message = message,
            },
            cancellationToken);
    }

    private static MailMessage CreateMailMessage(QueuedEmail email, MailSender sender)
    {
        MailMessage message = new()
        {
            IsBodyHtml = email.IsBodyHtml,
            Subject = email.Subject,
            Body = email.Content,
            From = CreateFromAddress(sender),
        };

        message.To.Add(email.To);

        if (!string.IsNullOrWhiteSpace(email.CC))
            message.CC.Add(email.CC);

        return message;
    }

    private static MailAddress CreateFromAddress(MailSender sender)
    {
        if (!string.IsNullOrWhiteSpace(sender.FromEmail))
            return new MailAddress(sender.FromEmail);

        return sender.User.Contains('@')
            ? new MailAddress(sender.User)
            : null;
    }
}
