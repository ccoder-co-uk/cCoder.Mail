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
    public SentEmail GetSentEmail(int sentEmailId) =>
        TryCatch<SentEmail>(operation: () =>
    {
        ValidateGet(inputs: [sentEmailId]);

        return processingService.GetSentEmail(iSentEmailId: sentEmailId);
    });

    public IQueryable<SentEmail> GetAllSentEmail(bool ignoreFilters = false) =>
        TryCatch<IQueryable<SentEmail>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return processingService.GetAllSentEmail(ignoreFilters: ignoreFilters);
    });

    public ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail) =>
        TryCatch<SentEmail>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newSentEmail]);

        SentEmail result = await processingService.AddSentEmailAsync(newSentEmail: newSentEmail);
        await eventService.RaiseSentEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail) =>
        TryCatch<SentEmail>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedSentEmail]);

        SentEmail result = await processingService.UpdateSentEmailAsync(updatedSentEmail: updatedSentEmail);
        await eventService.RaiseSentEmailUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int sentEmailId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [sentEmailId]);

        SentEmail entity = processingService.GetAllSentEmail(ignoreFilters: true)
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

    public ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdateSentEmailResult(IEnumerable<SentEmail> newSentEmail) =>
        TryCatch<IEnumerable<Result<SentEmail>>>(operation: () =>
    {
        ValidateAddOrUpdate(inputs: [newSentEmail]);

        return processingService.AddOrUpdateSentEmailResult(newSentEmail: newSentEmail);
    }, isValueTask: true);

    public ValueTask DeleteAllSentEmailAsync(IEnumerable<SentEmail> deletedSentEmail) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAllAsync(inputs: [deletedSentEmail]);

        return processingService.DeleteAllSentEmailAsync(deletedSentEmail: deletedSentEmail);
    }, isValueTask: true);
}