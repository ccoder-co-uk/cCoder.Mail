// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class MailServerOrchestrationService(IMailServerProcessingService processingService, IMailServerEventProcessingService eventService) : IMailServerOrchestrationService
{
    public MailServer Get(int id) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateGet(inputs: [id]);

        return processingService.Get(id: id);
    });

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailServer>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return processingService.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<MailServer> AddAsync(MailServer entity) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateAddAsync(inputs: [entity]);

        MailServer result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseMailServerAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateAsync(MailServer entity) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [entity]);

        MailServer result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseMailServerUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int id) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [id]);

        MailServer entity = processingService.GetAll(ignoreFilters: true)
                                                       .FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseMailServerDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdate(IEnumerable<MailServer> items) =>
        TryCatch<IEnumerable<Result<MailServer>>>(operation: () =>
    {
        ValidateAddOrUpdate(inputs: [items]);

        return processingService.AddOrUpdate(items: items);
    }, isValueTask: true);

    public ValueTask DeleteAllAsync(IEnumerable<MailServer> items) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAllAsync(inputs: [items]);

        return processingService.DeleteAllAsync(items: items);
    }, isValueTask: true);
}