using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class Pop3MailReceiverProvider(IMailClient mailClient) : IMailReceiverProvider
{
    public string ProviderName => MailProviderNames.Pop3;

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailClient.ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        mailClient.ReceiveTopAsync(count, cancellationToken);
}
