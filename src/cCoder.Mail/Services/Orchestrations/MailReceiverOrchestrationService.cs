using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal sealed class MailReceiverOrchestrationService(
    IMailReceiverProcessingService mailReceiverProcessingService,
    IReceivedEmailProcessingService receivedEmailProcessingService,
    IMailReceivingService mailReceivingService,
    ILogger<MailReceiverOrchestrationService> log)
    : IMailReceiverOrchestrationService
{
    public async Task RunContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await RunAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
            }
        }
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        MailReceiver[] receivers = mailReceiverProcessingService.GetEnabled();

        foreach (MailReceiver receiver in receivers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ReceiveAsync(receiver, cancellationToken);
        }
    }

    private async Task ReceiveAsync(MailReceiver receiver, CancellationToken cancellationToken)
    {
        DateTimeOffset from = receiver.LastReceivedOn ?? DateTimeOffset.UtcNow.AddDays(-1);
        DateTimeOffset to = DateTimeOffset.UtcNow;
        ReceivedEmail[] receivedEmails = await mailReceivingService.ReceiveAsync(
            new MailboxReceiveRequest
            {
                ProviderName = receiver.ProviderName,
                AppId = receiver.AppId,
                MailReceiverId = receiver.Id,
                Host = receiver.Host,
                Port = receiver.Port,
                EnableSSL = receiver.EnableSSL,
                User = receiver.User,
                Password = receiver.Password,
                From = from,
                To = to,
                MaximumMessages = 100,
            },
            cancellationToken);

        ReceivedEmail[] newEmails =
        [
            .. receivedEmails
                .Where(email => !receivedEmailProcessingService.Exists(receiver.Id, email.MessageId))
                .Select(email => Prepare(email, receiver))
        ];

        await receivedEmailProcessingService.AddRangeAsync(newEmails, cancellationToken);

        receiver.LastReceivedOn = to;
        await mailReceiverProcessingService.UpdateAsync(receiver);
    }

    private static ReceivedEmail Prepare(ReceivedEmail email, MailReceiver receiver)
    {
        email.AppId = receiver.AppId;
        email.MailReceiverId = receiver.Id;
        email.SentByUserId ??= "Guest";
        email.ReceivedOn = email.ReceivedOn == default ? DateTimeOffset.UtcNow : email.ReceivedOn;
        return email;
    }
}
