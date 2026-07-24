// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Orchestrations;

public interface IQueuedEmailOrchestrationService
{
    QueuedEmail GetQueuedEmail(int iQueuedEmailId);

    IQueryable<QueuedEmail> GetAllQueuedEmail(bool ignoreFilters = false);

    ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail);

    ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail);

    ValueTask DeleteAsync(int iQueuedEmailId);
    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdateQueuedEmailResult(IEnumerable<QueuedEmail> items);

    ValueTask DeleteAllQueuedEmailAsync(IEnumerable<QueuedEmail> items);

    ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail, bool checkPrivs);
}