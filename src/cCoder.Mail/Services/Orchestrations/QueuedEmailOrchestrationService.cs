// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class QueuedEmailOrchestrationService(IQueuedEmailProcessingService processingService, IQueuedEmailEventProcessingService eventService) : IQueuedEmailOrchestrationService
{
    public QueuedEmail Get(int id) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateGet(inputs: [id]);

        return processingService.Get(id: id);
    });

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<QueuedEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return processingService.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail entity) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateAddAsync(inputs: [entity]);

        QueuedEmail result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<QueuedEmail> UpdateAsync(QueuedEmail entity) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [entity]);

        QueuedEmail result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseQueuedEmailUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int id) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [id]);

        QueuedEmail entity = processingService.GetAll(ignoreFilters: true)
                                                       .FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseQueuedEmailDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdate(IEnumerable<QueuedEmail> items) =>
        TryCatch<IEnumerable<Result<QueuedEmail>>>(operation: () =>
    {
        ValidateAddOrUpdate(inputs: [items]);

        return processingService.AddOrUpdate(items: items);
    }, isValueTask: true);

    public ValueTask DeleteAllAsync(IEnumerable<QueuedEmail> items) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAllAsync(inputs: [items]);

        return processingService.DeleteAllAsync(items: items);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail entity, bool checkPrivs) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateAddAsync(inputs: [entity, checkPrivs]);

        QueuedEmail result = await processingService.AddAsync(entity: entity, checkPrivs: checkPrivs);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);
}