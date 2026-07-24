// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Processings;
using cCoder.Mail.Services.Orchestrations;


namespace cCoder.Mail.Services.Orchestrations;

internal sealed partial class MailSenderOrchestrationService(
    IQueuedEmailProcessingService queuedEmailProcessingService,
    IMailSendingProcessingService mailSendingProcessingService,
    IMailSenderProcessingService mailSenderProcessingService)
    : IMailSenderOrchestrationService
{
    public ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateMailSenderOnAdd(inputs: [newMailSender]);

            return mailSenderProcessingService.AddMailSenderAsync(
                newMailSender: newMailSender);
        }, isValueTask: true);

    public ValueTask<MailSender> UpdateMailSenderAsync(
        MailSender updatedMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateMailSenderOnUpdate(inputs: [updatedMailSender]);

            return mailSenderProcessingService.UpdateMailSenderAsync(
                updatedMailSender: updatedMailSender);
        }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateByAppIdOnDelete(inputs: [appId]);

            return mailSenderProcessingService.DeleteByAppIdAsync(
                appId: appId);
        }, isValueTask: true);

    public Task RunContinuouslyAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
    {

        ValidateRunContinuouslyAsync(inputs: [cancellationToken]);

        if (mailSendingProcessingService.IsMigrationInProgress())
        {
            return;
        }

        using PeriodicTimer timer = new(period: TimeSpan.FromMinutes(minutes: 1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            try
            {
                await RunOnceAsync(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                mailSendingProcessingService.LogError(exception: ex);
            }
        }
    }, isTask: true);

    public Task RunAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
    {
        ValidateRunAsync(inputs: [cancellationToken]);

        await RunOnceAsync(cancellationToken: cancellationToken);
    }, isTask: true);

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        QueuedEmail[] queue = queuedEmailProcessingService.GetDispatchBatch(
            batchSize: 10,
            maxFailures: 10);

        if (queue.Length == 0)
        {
            return;
        }

        mailSendingProcessingService.LogDispatch(count: queue.Length);

        int success = 0;
        int failures = 0;

        foreach (QueuedEmail email in queue)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await ProcessEmailAsync(email: email, cancellationToken: cancellationToken))
            {
                success++;
            }
            else
            {
                failures++;
            }

            await Task.Delay(millisecondsDelay: 500, cancellationToken: cancellationToken);
        }

        mailSendingProcessingService.LogSummary(
            count: success + failures,
            success: success,
            failures: failures);
    }

    private async Task<bool> ProcessEmailAsync(QueuedEmail email, CancellationToken cancellationToken)
    {
        MailSender sender = email.MailSender;

        if (sender == null)
        {
            await queuedEmailProcessingService.RecordSendFailureAsync(
emailId: email.Id,
reason: "No mail sender configuration could be found to send the email.",
cancellationToken: cancellationToken);

            return false;
        }

        try
        {
            await mailSendingProcessingService.SendQueuedEmailAsync(
                email: email,
                cancellationToken: cancellationToken);

            await queuedEmailProcessingService.MarkAsSentQueuedEmailAsync(
                queuedEmail: email,
                mailSenderId: sender.Id,
                fromAddress: sender.FromEmail ?? sender.User,
                cancellationToken: cancellationToken);

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

            await queuedEmailProcessingService.RecordSendFailureAsync(
                emailId: email.Id,
                reason: reason.ToString(),
                cancellationToken: cancellationToken);

            return false;
        }
    }
}