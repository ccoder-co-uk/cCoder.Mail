// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal class SentEmailOrchestrationService(ISentEmailProcessingService processingService, ISentEmailEventProcessingService eventService) : ISentEmailOrchestrationService
{
    public SentEmail Get(int id)
    {
        return processingService.Get(id: id);
    }

    public IQueryable<SentEmail> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask<SentEmail> AddAsync(SentEmail entity)
    {
        SentEmail result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseSentEmailAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<SentEmail> UpdateAsync(SentEmail entity)
    {
        SentEmail result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseSentEmailUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        SentEmail entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseSentEmailDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdate(IEnumerable<SentEmail> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<SentEmail> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}