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
    QueuedEmail Get(int iQueuedEmailId);
    IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false);
    QueuedEmail[] GetDispatchBatch(int batchSize, int maxFailures);
    ValueTask<QueuedEmail> AddAsync(QueuedEmail newQueuedEmail, bool checkPrivileges = true);
    ValueTask<QueuedEmail> UpdateAsync(QueuedEmail updatedQueuedEmail);
    ValueTask RecordSendFailureAsync(int emailId, string reason, CancellationToken cancellationToken = default);
    ValueTask MarkAsSentAsync(
        QueuedEmail queuedEmail,
        Guid mailSenderId,
        string fromAddress,
        CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(int iQueuedEmailId, bool checkPrivileges = true);
    ValueTask DeleteAllForAppAsync(IEnumerable<QueuedEmail> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}