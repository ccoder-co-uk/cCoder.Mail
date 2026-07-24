// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;


namespace cCoder.Mail.Services.Orchestrations;

internal sealed class MailSenderOrchestrationService(
    IQueuedEmailService queuedEmailService,
    IMailClientOrchestrationService mailClientOrchestrationService,
    MailConfiguration mailConfiguration,
    ILogger<MailSenderOrchestrationService> log)
    : IMailSenderOrchestrationService
{
    public async Task RunContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (mailConfiguration.IsMigrating)
            return;

        using PeriodicTimer timer = new(period: TimeSpan.FromMinutes(minutes: 1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            try
            {
                await RunAsync(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                log.LogError(exception: ex, message: ex.Message);
            }
        }
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        QueuedEmail[] queue = queuedEmailService.GetDispatchBatch(batchSize: 10, maxFailures: 10);

        if (queue.Length == 0)
            return;

        log.LogInformation(message: "Picked up a batch of {Count} emails.", args: queue.Length);

        int success = 0;
        int failures = 0;

        foreach (QueuedEmail email in queue)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await ProcessEmailAsync(email: email, cancellationToken: cancellationToken))
                success++;
            else
                failures++;

            await Task.Delay(millisecondsDelay: 500, cancellationToken: cancellationToken);
        }

        log.LogInformation(
message: "{Count} SMTP requests made of which {Success} succeeded and {Failures} failed.",
            success + failures,
            success,
            failures);
    }

    private async Task<bool> ProcessEmailAsync(QueuedEmail email, CancellationToken cancellationToken)
    {
        MailSender sender = email.MailSender;

        if (sender == null)
        {
            await queuedEmailService.RecordSendFailureAsync(
emailId: email.Id,
reason: "No mail sender configuration could be found to send the email.",
cancellationToken: cancellationToken);

            return false;
        }

        try
        {
            await mailClientOrchestrationService.SendAsync(email: email, cancellationToken: cancellationToken);
            await queuedEmailService.MarkAsSentAsync(queuedEmail: email, mailSenderId: sender.Id, fromAddress: sender.FromEmail ?? sender.User, cancellationToken: cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            StringBuilder reason = new(value: ex.Message);

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;

                _ = reason.Append(value: '\n')
                    .Append(value: ex.Message);
            }

            await queuedEmailService.RecordSendFailureAsync(emailId: email.Id, reason: reason.ToString(), cancellationToken: cancellationToken);
            return false;
        }
    }
}