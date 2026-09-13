// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailReceivingService(
    IMailReceiverClientBroker mailReceiverClientBroker,
    IMailConfigurationExposure mailConfigurationExposure,
    ILoggingBroker logger)
    : IMailReceivingService
{
    public bool IsMigrationInProgress() =>
        TryCatch(operation: () =>
            mailConfigurationExposure
                .GetMailConfiguration()
                .IsMigrating);

    public void LogError(Exception exception) =>
        TryCatch(operation: () =>
        {
            ValidateLogError(inputs: [exception]);

            logger.LogError(
                exception: exception,
                message: exception.Message);
        });

    public Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest mailboxReceiveRequest,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveMailboxReceiveRequestAsync(inputs: [mailboxReceiveRequest, cancellationToken]);

            return mailReceiverClientBroker.ReceiveAsync(request: mailboxReceiveRequest, cancellationToken: cancellationToken);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        Guid mailReceiverId,
        int count,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveTopAsync(
                inputs:
                    [
                        mailReceiverId,
                        count,
                        cancellationToken
                    ]);

            return mailReceiverClientBroker.ReceiveTopAsync(
                mailReceiverId: mailReceiverId,
                count: count,
                cancellationToken: cancellationToken);
        }, isTask: true);
}