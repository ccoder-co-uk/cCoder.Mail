using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Orchestrations;

internal sealed class MailClientOrchestrationService(
    IMailSendingService mailSendingService,
    IMailReceivingService mailReceivingService)
    : IMailClientOrchestrationService
{
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        mailSendingService.SendAsync(email, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailReceivingService.ReceiveAsync(request, cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        mailReceivingService.ReceiveTopAsync(count, cancellationToken);
}
