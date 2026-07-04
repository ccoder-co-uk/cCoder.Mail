using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailReceiverConfigurationOrchestrationService
{
    MailReceiver Get(Guid id);
    IQueryable<MailReceiver> GetAll(bool ignoreFilters = false);
    ValueTask<MailReceiver> AddAsync(MailReceiver entity);
    ValueTask<MailReceiver> UpdateAsync(MailReceiver entity);
    ValueTask<int> DeleteAsync(Guid id);
    ValueTask DeleteByAppIdAsync(int appId);
}
