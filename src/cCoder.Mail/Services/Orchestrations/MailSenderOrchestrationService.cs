using System.Text;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;


namespace cCoder.Mail.Services.Orchestrations;

internal sealed class MailSenderOrchestrationService(
    IQueuedEmailService queuedEmailService,
    IMailClientOrchestrationService mailClientOrchestrationService,
    ILogger<MailSenderOrchestrationService> log)
    : IMailSenderOrchestrationService
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
        QueuedEmail[] queue = queuedEmailService.GetDispatchBatch(batchSize: 10, maxFailures: 10);

        if (queue.Length == 0)
            return;

        log.LogInformation("Picked up a batch of {Count} emails.", queue.Length);

        int success = 0;
        int failures = 0;

        foreach (QueuedEmail email in queue)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await ProcessEmailAsync(email, cancellationToken))
                success++;
            else
                failures++;

            await Task.Delay(500, cancellationToken);
        }

        log.LogInformation(
            "{Count} SMTP requests made of which {Success} succeeded and {Failures} failed.",
            success + failures,
            success,
            failures);
    }

    private async Task<bool> ProcessEmailAsync(QueuedEmail email, CancellationToken cancellationToken)
    {
        MailServer server = email.App?.MailServers?.FirstOrDefault(mailServer => mailServer.Name == email.MailServerName);

        if (server == null)
        {
            await queuedEmailService.RecordSendFailureAsync(
                email.Id,
                "No mail server configuration could be found to send the email.",
                cancellationToken);
            return false;
        }

        try
        {
            await mailClientOrchestrationService.SendAsync(email, cancellationToken);
            await queuedEmailService.MarkAsSentAsync(email, server.User, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            StringBuilder reason = new(ex.Message);

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                _ = reason.Append('\n').Append(ex.Message);
            }

            await queuedEmailService.RecordSendFailureAsync(email.Id, reason.ToString(), cancellationToken);
            return false;
        }
    }
}
