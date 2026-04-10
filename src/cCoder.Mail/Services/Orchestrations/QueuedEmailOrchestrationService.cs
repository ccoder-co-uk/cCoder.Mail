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
        return processingService.Get(id);
    }

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<QueuedEmail> AddAsync(QueuedEmail entity)
    {
        QueuedEmail result = await processingService.AddAsync(entity);
        await eventService.RaiseQueuedEmailAddEventAsync(result);
        return result;
    }

    public async ValueTask<QueuedEmail> UpdateAsync(QueuedEmail entity)
    {
        QueuedEmail result = await processingService.UpdateAsync(entity);
        await eventService.RaiseQueuedEmailUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        QueuedEmail entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseQueuedEmailDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        QueuedEmail[] queuedEmails = [.. processingService.GetAll(ignoreFilters: true).Where(item => item.AppId == appId)];

        foreach (QueuedEmail queuedEmail in queuedEmails)
            await DeleteAsync(queuedEmail.Id);
    }

    public ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdate(IEnumerable<QueuedEmail> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<QueuedEmail> items)
    {
        return processingService.DeleteAllAsync(items);
    }

    public async ValueTask<QueuedEmail> AddAsync(QueuedEmail entity, bool checkPrivs)
    {
        QueuedEmail result = await processingService.AddAsync(entity, checkPrivs);
        await eventService.RaiseQueuedEmailAddEventAsync(result);
        return result;
    }
}
