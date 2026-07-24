// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations;

public interface IQueuedEmailService
{
    QueuedEmail GetQueuedEmail(int iQueuedEmailId);
    IQueryable<QueuedEmail> GetAllQueuedEmail(bool ignoreFilters = false);
    QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures);
    ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail, bool checkPrivileges = true);
    ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail);
    ValueTask RecordSendFailureAsync(int emailId, string reason, CancellationToken cancellationToken = default);
    ValueTask MarkAsSentQueuedEmailAsync(
        QueuedEmail queuedEmail,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(int iQueuedEmailId, bool checkPrivileges = true);
    ValueTask DeleteAllForAppQueuedEmailAsync(IEnumerable<QueuedEmail> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}