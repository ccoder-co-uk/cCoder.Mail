using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations;

public interface IMailServerService
{
    MailServer Get(int id);
    IQueryable<MailServer> GetAll(bool ignoreFilters = false);
    ValueTask<MailServer> AddAsync(MailServer mailServer);
    ValueTask<MailServer> UpdateAsync(MailServer mailServer);
    ValueTask DeleteAsync(int id);
    ValueTask DeleteAllForAppAsync(IEnumerable<MailServer> items);
    ValueTask DeleteAllByAppIdAsync(int appId);
}









