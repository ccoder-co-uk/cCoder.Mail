using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class Pop3MailReceiverProvider(IPop3MailReceiverService pop3MailReceiverService)
    : IMailReceiverProvider
{
    public string ProviderName => MailProviderNames.Pop3;

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        pop3MailReceiverService.ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        pop3MailReceiverService.ReceiveTopAsync(count, cancellationToken);
}
