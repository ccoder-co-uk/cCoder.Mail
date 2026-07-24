// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal class QueuedEmailOrchestrationService(IQueuedEmailProcessingService processingService, IQueuedEmailEventProcessingService eventService) : IQueuedEmailOrchestrationService
{
    public QueuedEmail Get(int id)
    {
        return processingService.Get(id: id);
    }

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask<QueuedEmail> AddAsync(QueuedEmail entity)
    {
        QueuedEmail result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<QueuedEmail> UpdateAsync(QueuedEmail entity)
    {
        QueuedEmail result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseQueuedEmailUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        QueuedEmail entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseQueuedEmailDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdate(IEnumerable<QueuedEmail> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<QueuedEmail> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }

    public async ValueTask<QueuedEmail> AddAsync(QueuedEmail entity, bool checkPrivs)
    {
        QueuedEmail result = await processingService.AddAsync(entity: entity, checkPrivs: checkPrivs);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }
}