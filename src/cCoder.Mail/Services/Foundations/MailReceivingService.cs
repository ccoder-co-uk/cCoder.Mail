using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed class MailReceivingService(IMailClientBroker mailClientBroker) : IMailReceivingService
{
    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailClientBroker.ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        mailClientBroker.ReceiveTopAsync(count, cancellationToken);
}
