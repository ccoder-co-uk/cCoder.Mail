using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed class SmtpMailSenderService(ISmtpMailSenderBroker smtpMailSenderBroker)
    : ISmtpMailSenderService
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default)
    {
        MailSender sender = email.MailSender
            ?? throw new InvalidOperationException("No mail sender configuration could be found to send the email.");

        return smtpMailSenderBroker.SendAsync(
            new SmtpMailSendRequest
            {
                Host = sender.Host,
                Port = sender.Port,
                EnableSsl = sender.EnableSSL,
                User = sender.User,
                Password = sender.Password,
                From = sender.FromEmail,
                To = email.To,
                CC = email.CC,
                Subject = email.Subject,
                Content = email.Content,
                IsBodyHtml = email.IsBodyHtml,
            },
            cancellationToken);
    }
}
