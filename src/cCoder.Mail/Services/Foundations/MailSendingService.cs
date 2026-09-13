// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailSendingService(
    IMailSenderClientBroker mailSenderClientBroker,
    IMailConfigurationExposure mailConfigurationExposure,
    ILoggingBroker logger)
    : IMailSendingService
{
    public bool IsMigrationInProgress() =>
        TryCatch(operation: () =>
            mailConfigurationExposure
                .GetMailConfiguration()
                .IsMigrating);

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

    public Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateSendQueuedEmailAsync(inputs: [email, cancellationToken]);

            return mailSenderClientBroker.SendAsync(email: email, cancellationToken: cancellationToken);
        }, isTask: true);
}