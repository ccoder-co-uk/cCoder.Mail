// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal class MailServerOrchestrationService(IMailServerProcessingService processingService, IMailServerEventProcessingService eventService) : IMailServerOrchestrationService
{
    public MailServer Get(int id)
    {
        return processingService.Get(id: id);
    }

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask<MailServer> AddAsync(MailServer entity)
    {
        MailServer result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseMailServerAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<MailServer> UpdateAsync(MailServer entity)
    {
        MailServer result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseMailServerUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        MailServer entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseMailServerDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdate(IEnumerable<MailServer> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<MailServer> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}