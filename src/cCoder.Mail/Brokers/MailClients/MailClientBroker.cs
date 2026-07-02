using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailClientBroker(
    IMailSenderFactory mailSenderFactory,
    IMailReceiverFactory mailReceiverFactory) : IMailClientBroker
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailSenderFactory
            .GetSender(GetSenderProviderName(email))
            .SendAsync(email, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailReceiverFactory
            .GetReceiver(request?.ProviderName)
            .ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        mailReceiverFactory
            .GetReceiver(null)
            .ReceiveTopAsync(count, cancellationToken);

    private static string GetSenderProviderName(QueuedEmail email)
    {
        MailSender sender = email?.App?.MailSenders?.FirstOrDefault(
            mailSender => mailSender.Name == email.MailServerName);

        return sender?.ProviderName;
    }
}
