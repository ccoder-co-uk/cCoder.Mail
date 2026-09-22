// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal sealed partial class MailReceivingProcessingService(
    IMailReceivingService mailReceivingService)
    : IMailReceivingProcessingService
{
    public bool IsMigrationInProgress() =>
        TryCatch(operation: () =>
        {
            return mailReceivingService.IsMigrationInProgress();
        });

    public void LogError(Exception exception) =>
        TryCatch(operation: () =>
        {
            ValidateLogError(inputs: [exception]);

            mailReceivingService.LogError(exception: exception);
        });

    public Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest mailboxReceiveRequest,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveMailboxReceiveRequestAsync(
                inputs: [mailboxReceiveRequest, cancellationToken]);

            return mailReceivingService.ReceiveMailboxReceiveRequestAsync(
                request: mailboxReceiveRequest,
                cancellationToken: cancellationToken);
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

            return mailReceivingService.ReceiveTopAsync(
                mailReceiverId: mailReceiverId,
                count: count,
                cancellationToken: cancellationToken);
        }, isTask: true);
}