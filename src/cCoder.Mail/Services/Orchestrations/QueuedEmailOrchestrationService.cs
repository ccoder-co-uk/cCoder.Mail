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
    public QueuedEmail GetQueuedEmail(int queuedEmailId) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateQueuedEmailOnGet(inputs: [queuedEmailId]);

        return processingService.GetQueuedEmail(iQueuedEmailId: queuedEmailId);
    });

    public IQueryable<QueuedEmail> GetAllQueuedEmail(bool ignoreFilters = false) =>
        TryCatch<IQueryable<QueuedEmail>>(operation: () =>
    {
        ValidateAllQueuedEmailOnGet(inputs: [ignoreFilters]);

        return processingService.GetAllQueuedEmail(ignoreFilters: ignoreFilters);
    });

    public ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateQueuedEmailOnAdd(inputs: [newQueuedEmail]);

        QueuedEmail result = await processingService.AddQueuedEmailAsync(newQueuedEmail: newQueuedEmail);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateQueuedEmailOnUpdate(inputs: [updatedQueuedEmail]);

        QueuedEmail result = await processingService.UpdateQueuedEmailAsync(updatedQueuedEmail: updatedQueuedEmail);
        await eventService.RaiseQueuedEmailUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int queuedEmailId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [queuedEmailId]);

        QueuedEmail entity = processingService.GetAllQueuedEmail(ignoreFilters: true)
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
            ValidateByAppIdOnDelete(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdateQueuedEmailResult(IEnumerable<QueuedEmail> newQueuedEmail) =>
        TryCatch<IEnumerable<Result<QueuedEmail>>>(operation: () =>
    {
        ValidateOrUpdateQueuedEmailResultOnAdd(inputs: [newQueuedEmail]);

        return processingService.AddOrUpdateQueuedEmailResult(newQueuedEmail: newQueuedEmail);
    }, isValueTask: true);

    public ValueTask DeleteAllQueuedEmailAsync(IEnumerable<QueuedEmail> deletedQueuedEmail) =>
        TryCatch(operation: () =>
    {
        ValidateAllQueuedEmailOnDelete(inputs: [deletedQueuedEmail]);

        return processingService.DeleteAllQueuedEmailAsync(deletedQueuedEmail: deletedQueuedEmail);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail, bool checkPrivs) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateQueuedEmailOnAdd(inputs: [newQueuedEmail, checkPrivs]);

        QueuedEmail result = await processingService.AddQueuedEmailAsync(newQueuedEmail: newQueuedEmail, checkPrivs: checkPrivs);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);
}