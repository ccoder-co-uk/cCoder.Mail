using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailClientBroker(IMailClient mailClient) : IMailClientBroker
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailClient.SendAsync(email, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailClient.ReceiveAsync(request, cancellationToken);
}
