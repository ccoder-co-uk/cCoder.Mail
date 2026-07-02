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
    ValueTask DeleteAllAsync(IEnumerable<MailReceiver> items);
}
