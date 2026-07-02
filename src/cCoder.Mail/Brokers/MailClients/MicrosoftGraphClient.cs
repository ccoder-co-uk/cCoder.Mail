using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MicrosoftGraphClient(
    IMicrosoftGraphMailSenderService microsoftGraphMailSenderService,
    IMicrosoftGraphMailReceiverService microsoftGraphMailReceiverService)
    : IMicrosoftGraphClient, IMailSenderProvider, IMailReceiverProvider
{
    public string ProviderName => MailProviderNames.MicrosoftGraph;

    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        microsoftGraphMailSenderService.SendAsync(email, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        microsoftGraphMailReceiverService.ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        microsoftGraphMailReceiverService.ReceiveTopAsync(count, cancellationToken);
}
