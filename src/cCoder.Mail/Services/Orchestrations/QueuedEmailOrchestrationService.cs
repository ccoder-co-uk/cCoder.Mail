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
    public QueuedEmail Get(int queuedEmailId) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateGet(inputs: [queuedEmailId]);

        return processingService.Get(iQueuedEmailId: queuedEmailId);
    });

    public IQueryable<QueuedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<QueuedEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return processingService.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail newQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newQueuedEmail]);

        QueuedEmail result = await processingService.AddAsync(newQueuedEmail: newQueuedEmail);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<QueuedEmail> UpdateAsync(QueuedEmail updatedQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedQueuedEmail]);

        QueuedEmail result = await processingService.UpdateAsync(updatedQueuedEmail: updatedQueuedEmail);
        await eventService.RaiseQueuedEmailUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int queuedEmailId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [queuedEmailId]);

        QueuedEmail entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == queuedEmailId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseQueuedEmailDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(iQueuedEmailId: queuedEmailId);
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

    public ValueTask<QueuedEmail> AddAsync(QueuedEmail newQueuedEmail, bool checkPrivs) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newQueuedEmail, checkPrivs]);

        QueuedEmail result = await processingService.AddAsync(newQueuedEmail: newQueuedEmail, checkPrivs: checkPrivs);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);
}