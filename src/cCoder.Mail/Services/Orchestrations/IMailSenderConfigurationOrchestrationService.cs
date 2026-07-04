using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailSenderConfigurationOrchestrationService
{
    MailSender Get(Guid id);
    IQueryable<MailSender> GetAll(bool ignoreFilters = false);
    ValueTask<MailSender> AddAsync(MailSender entity);
    ValueTask<MailSender> UpdateAsync(MailSender entity);
    ValueTask<int> DeleteAsync(Guid id);
    ValueTask DeleteByAppIdAsync(int appId);
}
