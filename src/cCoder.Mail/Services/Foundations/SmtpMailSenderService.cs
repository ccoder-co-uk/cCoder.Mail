using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;

namespace cCoder.Mail.Services.Foundations;

internal sealed class SmtpMailSenderService(ISmtpMailSenderBroker smtpMailSenderBroker)
    : ISmtpMailSenderService
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        smtpMailSenderBroker.SendAsync(email, cancellationToken);
}
