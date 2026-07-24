// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Processings;

public interface IQueuedEmailProcessingService
{
    QueuedEmail Get(int iQueuedEmailId);

    IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false);

    ValueTask<QueuedEmail> AddAsync(QueuedEmail newQueuedEmail);

    ValueTask<QueuedEmail> UpdateAsync(QueuedEmail updatedQueuedEmail);

    ValueTask DeleteAsync(int iQueuedEmailId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdate(IEnumerable<QueuedEmail> items);

    ValueTask DeleteAllAsync(IEnumerable<QueuedEmail> items);

    ValueTask<QueuedEmail> AddAsync(QueuedEmail newQueuedEmail, bool checkPrivs);
}