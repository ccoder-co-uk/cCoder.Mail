// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Foundations;

internal interface IMailSendingService
{
    bool IsMigrationInProgress();

    void LogDispatch(int count);

    void LogSummary(int count, int success, int failures);

    void LogError(Exception exception);

    Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default);
}