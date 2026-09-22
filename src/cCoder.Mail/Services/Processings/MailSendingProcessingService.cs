// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal sealed partial class MailSendingProcessingService(
    IMailSendingService mailSendingService)
    : IMailSendingProcessingService
{
    public bool IsMigrationInProgress() =>
        TryCatch(operation: () =>
        {
            return mailSendingService.IsMigrationInProgress();
        });

    public void LogDispatch(int count) =>
        TryCatch(operation: () =>
        {
            ValidateLogDispatch(inputs: [count]);

            mailSendingService.LogDispatch(count: count);
        });

    public void LogSummary(int count, int success, int failures) =>
        TryCatch(operation: () =>
        {
            ValidateLogSummary(inputs: [count, success, failures]);

            mailSendingService.LogSummary(
                count: count,
                success: success,
                failures: failures);
        });

    public void LogError(Exception exception) =>
        TryCatch(operation: () =>
        {
            ValidateLogError(inputs: [exception]);

            mailSendingService.LogError(exception: exception);
        });

    public Task SendQueuedEmailAsync(
        QueuedEmail email,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateSendQueuedEmailAsync(
                inputs: [email, cancellationToken]);

            return mailSendingService.SendQueuedEmailAsync(
                email: email,
                cancellationToken: cancellationToken);
        }, isTask: true);
}