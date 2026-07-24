// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Exposures;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal sealed partial class MailReceivingProcessingService(
    IMailReceivingService mailReceivingService,
    IMailConfigurationExposure mailConfigurationExposure,
    ILogger<MailReceivingProcessingService> logger)
    : IMailReceivingProcessingService
{
    public bool IsMigrationInProgress() =>
        TryCatch(operation: () =>
        {
            ValidateIsMigrationInProgress(inputs: []);

            return mailConfigurationExposure
                .GetMailConfiguration()
                .IsMigrating;
        });

    public void LogError(Exception exception) =>
        TryCatch(operation: () =>
        {
            ValidateLogError(inputs: [exception]);

            logger.LogError(
                exception: exception,
                message: exception.Message);
        });

    public Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveMailboxReceiveRequestAsync(
                inputs: [request, cancellationToken]);

            return mailReceivingService.ReceiveMailboxReceiveRequestAsync(
                request: request,
                cancellationToken: cancellationToken);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveTopAsync(
                inputs: [count, cancellationToken]);

            return mailReceivingService.ReceiveTopAsync(
                count: count,
                cancellationToken: cancellationToken);
        }, isTask: true);
}