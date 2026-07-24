// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class SentEmailOrchestrationService(ISentEmailProcessingService processingService, ISentEmailEventProcessingService eventService) : ISentEmailOrchestrationService
{
    public SentEmail Get(int id) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateGet(inputs: [id]);

        return processingService.Get(id: id);
    });

    public IQueryable<SentEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<SentEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return processingService.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<SentEmail> AddAsync(SentEmail entity) =>
        TryCatch<SentEmail>(operation: async () =>
    {
        ValidateAddAsync(inputs: [entity]);

        SentEmail result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseSentEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<SentEmail> UpdateAsync(SentEmail entity) =>
        TryCatch<SentEmail>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [entity]);

        SentEmail result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseSentEmailUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int id) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [id]);

        SentEmail entity = processingService.GetAll(ignoreFilters: true)
                                                       .FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseSentEmailDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdate(IEnumerable<SentEmail> items) =>
        TryCatch<IEnumerable<Result<SentEmail>>>(operation: () =>
    {
        ValidateAddOrUpdate(inputs: [items]);

        return processingService.AddOrUpdate(items: items);
    }, isValueTask: true);

    public ValueTask DeleteAllAsync(IEnumerable<SentEmail> items) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAllAsync(inputs: [items]);

        return processingService.DeleteAllAsync(items: items);
    }, isValueTask: true);
}