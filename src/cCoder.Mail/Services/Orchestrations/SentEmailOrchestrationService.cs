using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal class SentEmailOrchestrationService(ISentEmailProcessingService processingService, ISentEmailEventProcessingService eventService) : ISentEmailOrchestrationService
{
    public SentEmail Get(int id)
    {
        return processingService.Get(id);
    }

    public IQueryable<SentEmail> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<SentEmail> AddAsync(SentEmail entity)
    {
        SentEmail result = await processingService.AddAsync(entity);
        await eventService.RaiseSentEmailAddEventAsync(result);
        return result;
    }

    public async ValueTask<SentEmail> UpdateAsync(SentEmail entity)
    {
        SentEmail result = await processingService.UpdateAsync(entity);
        await eventService.RaiseSentEmailUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        SentEmail entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseSentEmailDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        SentEmail[] sentEmails = [.. processingService.GetAll(ignoreFilters: true).Where(item => item.AppId == appId)];

        foreach (SentEmail sentEmail in sentEmails)
            await DeleteAsync(sentEmail.Id);
    }

    public ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdate(IEnumerable<SentEmail> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<SentEmail> items)
    {
        return processingService.DeleteAllAsync(items);
    }
}
