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
        ValidateMailServerOnGet(inputs: [mailServerId]);

        return processingService.GetMailServer(iMailServerId: mailServerId);
    });

    public IQueryable<MailServer> GetAllMailServer(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailServer>>(operation: () =>
    {
        ValidateAllMailServerOnGet(inputs: [ignoreFilters]);

        return processingService.GetAllMailServer(ignoreFilters: ignoreFilters);
    });

    public ValueTask<MailServer> AddMailServerAsync(MailServer newMailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateMailServerOnAdd(inputs: [newMailServer]);

        MailServer result = await processingService.AddMailServerAsync(newMailServer: newMailServer);
        await eventService.RaiseMailServerAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<MailServer> UpdateMailServerAsync(MailServer updatedMailServer) =>
        TryCatch<MailServer>(operation: async () =>
    {
        ValidateMailServerOnUpdate(inputs: [updatedMailServer]);

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
            ValidateByAppIdOnDelete(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);

    ValueTask<IEnumerable<Result<MailServer>>>
        IMailServerOrchestrationService.AddOrUpdateMailServerResult(
            IEnumerable<MailServer> newMailServer) =>
        TryCatch<IEnumerable<Result<MailServer>>>(operation: () =>
    {
        ValidateOrUpdateMailServerResultOnAdd(inputs: [newMailServer]);

        return processingService.AddOrUpdateMailServerResult(newMailServer: newMailServer);
    }, isValueTask: true);

    public ValueTask DeleteAllMailServerAsync(IEnumerable<MailServer> deletedMailServer) =>
        TryCatch(operation: () =>
    {
        ValidateAllMailServerOnDelete(inputs: [deletedMailServer]);

        return processingService.DeleteAllMailServerAsync(deletedMailServer: deletedMailServer);
    }, isValueTask: true);
}