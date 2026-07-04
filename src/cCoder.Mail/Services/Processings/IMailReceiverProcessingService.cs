using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Processings;

public interface IMailReceiverProcessingService
{
    MailReceiver Get(Guid id);
    IQueryable<MailReceiver> GetAll(bool ignoreFilters = false);
    MailReceiver[] GetEnabled();
    ValueTask<MailReceiver> AddAsync(MailReceiver entity);
    ValueTask<MailReceiver> UpdateAsync(MailReceiver entity);
    ValueTask<int> DeleteAsync(Guid id);
    ValueTask DeleteByAppIdAsync(int appId);
    ValueTask DeleteAllAsync(IEnumerable<MailReceiver> items);
}
