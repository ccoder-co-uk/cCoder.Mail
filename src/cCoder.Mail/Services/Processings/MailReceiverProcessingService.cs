using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal class MailReceiverProcessingService(IMailReceiverService service) : IMailReceiverProcessingService
{
    public MailReceiver Get(Guid id) => service.Get(id);

    public IQueryable<MailReceiver> GetAll(bool ignoreFilters = false) => service.GetAll(ignoreFilters);

    public MailReceiver[] GetEnabled() => service.GetEnabled();

    public ValueTask<MailReceiver> AddAsync(MailReceiver entity) => service.AddAsync(entity);

    public ValueTask<MailReceiver> UpdateAsync(MailReceiver entity) => service.UpdateAsync(entity);

    public ValueTask<int> DeleteAsync(Guid id) => service.DeleteAsync(id);

    public ValueTask DeleteAllAsync(IEnumerable<MailReceiver> items) => service.DeleteAllAsync(items);
}
