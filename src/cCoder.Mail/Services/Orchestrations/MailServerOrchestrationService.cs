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
    public MailServer GetMailServer(int mailServerId) =>
        TryCatch<MailServer>(operation: () =>
    {
        ValidateGet(inputs: [mailServerId]);

        return processingService.GetMailServer(iMailServerId: mailServerId);
    });

    public IQueryable<MailServer> GetAllMailServer(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailServer>>(operation: () =>
    {
        ValidateGetAll(inputs: [ignoreFilters]);

        return processingService.GetAllMailServer(ignoreFilters: ignoreFilters);
    });

    public ValueTask<MailServer> AddMailServerAsync(MailServer newMailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateAddAsync(inputs: [newMailServer]);

        MailServer result = await processingService.AddMailServerAsync(newMailServer: newMailServer);
        await eventService.RaiseMailServerAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedMailServer]);

        MailServer result = await processingService.UpdateMailServerAsync(updatedMailServer: updatedMailServer);
        await eventService.RaiseMailServerUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int mailServerId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [mailServerId]);

        MailServer entity = processingService.GetAllMailServer(ignoreFilters: true)
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

    public ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdateMailServerResult(IEnumerable<MailServer> newMailServer) =>
        TryCatch<IEnumerable<Result<MailServer>>>(operation: () =>
    {
        ValidateAddOrUpdate(inputs: [newMailServer]);

        return processingService.AddOrUpdateMailServerResult(newMailServer: newMailServer);
    }, isValueTask: true);

    public ValueTask DeleteAllMailServerAsync(IEnumerable<MailServer> deletedMailServer) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAllAsync(inputs: [deletedMailServer]);

        return processingService.DeleteAllMailServerAsync(deletedMailServer: deletedMailServer);
    }, isValueTask: true);
}