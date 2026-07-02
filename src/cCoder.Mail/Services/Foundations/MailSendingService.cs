using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;

namespace cCoder.Mail.Services.Foundations;

internal sealed class MailSendingService(IMailClientBroker mailClientBroker) : IMailSendingService
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailClientBroker.SendAsync(email, cancellationToken);
}
