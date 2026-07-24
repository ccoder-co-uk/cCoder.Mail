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
    public MailServer Get(int mailServerId) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateGet(inputs: [mailServerId]);

        return processingService.Get(iMailServerId: mailServerId);
    });

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailServer>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return processingService.GetAll(ignoreFilters: ignoreFilters);
    });

    public ValueTask<MailServer> AddAsync(MailServer newMailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newMailServer]);

        MailServer result = await processingService.AddAsync(newMailServer: newMailServer);
        await eventService.RaiseMailServerAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateAsync(MailServer updatedMailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedMailServer]);

        MailServer result = await processingService.UpdateAsync(updatedMailServer: updatedMailServer);
        await eventService.RaiseMailServerUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int mailServerId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [mailServerId]);

        MailServer entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == mailServerId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseMailServerDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(iMailServerId: mailServerId);
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