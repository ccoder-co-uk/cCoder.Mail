// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Exposures;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal sealed partial class MailSendingProcessingService(
    IMailSendingService mailSendingService,
    IMailConfigurationExposure mailConfigurationExposure,
    ILogger<MailSendingProcessingService> logger)
    : IMailSendingProcessingService
{
    public bool IsMigrationInProgress() =>
        TryCatch(operation: () =>
        {
            ValidateIsMigrationInProgress(inputs: []);

            return mailConfigurationExposure
                .GetMailConfiguration()
                .IsMigrating;
        });

    public void LogDispatch(int count) =>
        TryCatch(operation: () =>
        {
            ValidateLogDispatch(inputs: [count]);

            logger.LogInformation(
                message: "Picked up a batch of {Count} emails.",
                args: count);
        });

    public void LogSummary(int count, int success, int failures) =>
        TryCatch(operation: () =>
        {
            ValidateLogSummary(inputs: [count, success, failures]);

            logger.LogInformation(
                message: "{Count} SMTP requests made of which {Success} succeeded and {Failures} failed.",
                args: [count, success, failures]);
        });

    public void LogError(Exception exception) =>
        TryCatch(operation: () =>
        {
            ValidateLogError(inputs: [exception]);

            logger.LogError(
                exception: exception,
                message: exception.Message);
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