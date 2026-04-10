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
        return processingService.Get(id);
    }

    public IQueryable<MailServer> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<MailServer> AddAsync(MailServer entity)
    {
        MailServer result = await processingService.AddAsync(entity);
        await eventService.RaiseMailServerAddEventAsync(result);
        return result;
    }

    public async ValueTask<MailServer> UpdateAsync(MailServer entity)
    {
        MailServer result = await processingService.UpdateAsync(entity);
        await eventService.RaiseMailServerUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        MailServer entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseMailServerDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        MailServer[] mailServers = [.. processingService.GetAll(ignoreFilters: true).Where(item => item.AppId == appId)];

        foreach (MailServer mailServer in mailServers)
            await DeleteAsync(mailServer.Id);
    }

    public ValueTask<IEnumerable<Result<MailServer>>> AddOrUpdate(IEnumerable<MailServer> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<MailServer> items)
    {
        return processingService.DeleteAllAsync(items);
    }
}
