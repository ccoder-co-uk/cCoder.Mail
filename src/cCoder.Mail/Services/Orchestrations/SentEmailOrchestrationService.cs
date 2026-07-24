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
    public SentEmail Get(int sentEmailId) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateGet(inputs: [sentEmailId]);

        return processingService.Get(iSentEmailId: sentEmailId);
    });

    public IQueryable<SentEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<SentEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return processingService.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<SentEmail> AddAsync(SentEmail newSentEmail) =>
        TryCatch<SentEmail>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newSentEmail]);

        SentEmail result = await processingService.AddAsync(newSentEmail: newSentEmail);
        await eventService.RaiseSentEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<SentEmail> UpdateAsync(SentEmail updatedSentEmail) =>
        TryCatch<SentEmail>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedSentEmail]);

        SentEmail result = await processingService.UpdateAsync(updatedSentEmail: updatedSentEmail);
        await eventService.RaiseSentEmailUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int sentEmailId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [sentEmailId]);

        SentEmail entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == sentEmailId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseSentEmailDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(iSentEmailId: sentEmailId);
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