using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Exposures.MailClients;

internal sealed class SmtpMailSenderProvider(ISmtpMailSenderService smtpMailSenderService)
    : IMailSenderProvider
{
    public string ProviderName => MailProviderNames.Smtp;

    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        smtpMailSenderService.SendAsync(email, cancellationToken);
}
