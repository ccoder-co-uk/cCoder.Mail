using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations;

public interface ISentEmailService
{
    SentEmail Get(int id);
    IQueryable<SentEmail> GetAll(bool ignoreFilters = false);
    ValueTask<SentEmail> AddAsync(SentEmail sentEmail);
    ValueTask<SentEmail> UpdateAsync(SentEmail sentEmail);
    ValueTask DeleteAsync(int id);
}









