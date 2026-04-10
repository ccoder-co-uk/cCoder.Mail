using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Processings;

public interface ISentEmailProcessingService
{
    SentEmail Get(int id);

    IQueryable<SentEmail> GetAll(bool ignoreFilters = false);

    ValueTask<SentEmail> AddAsync(SentEmail entity);

    ValueTask<SentEmail> UpdateAsync(SentEmail entity);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<SentEmail>>> AddOrUpdate(IEnumerable<SentEmail> items);

    ValueTask DeleteAllAsync(IEnumerable<SentEmail> items);
}
