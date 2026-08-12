// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures;

public interface IQueuedEmailManager
{
    QueuedEmail GetQueuedEmail(int iQueuedEmailId);
    IQueryable<QueuedEmail> GetAllQueuedEmail(bool ignoreFilters = false);
    ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail);
    ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail);
    ValueTask DeleteAsync(int iQueuedEmailId);
    ValueTask RetryAsync(int queuedEmailId);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdateQueuedEmailResult(IEnumerable<QueuedEmail> newQueuedEmail);
    ValueTask DeleteAllQueuedEmailAsync(IEnumerable<QueuedEmail> deletedQueuedEmail);
    ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail, bool checkPrivs);
}