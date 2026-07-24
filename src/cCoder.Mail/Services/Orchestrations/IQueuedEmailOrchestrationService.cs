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
    QueuedEmail Get(int id);

    IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false);

    ValueTask<QueuedEmail> AddAsync(QueuedEmail entity);

    ValueTask<QueuedEmail> UpdateAsync(QueuedEmail entity);

    ValueTask DeleteAsync(int id);
    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdate(IEnumerable<QueuedEmail> items);

    ValueTask DeleteAllAsync(IEnumerable<QueuedEmail> items);

    ValueTask<QueuedEmail> AddAsync(QueuedEmail entity, bool checkPrivs);
}