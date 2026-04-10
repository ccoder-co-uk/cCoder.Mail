using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Orchestrations;

public interface ISentEmailOrchestrationService
{
    SentEmail Get(int id);

    IQueryable<SentEmail> GetAll(bool ignoreFilters = false);

    ValueTask<SentEmail> AddAsync(SentEmail entity);

    ValueTask<SentEmail> UpdateAsync(SentEmail entity);

    ValueTask DeleteAsync(int id);
    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdate(IEnumerable<SentEmail> items);

    ValueTask DeleteAllAsync(IEnumerable<SentEmail> items);
}
