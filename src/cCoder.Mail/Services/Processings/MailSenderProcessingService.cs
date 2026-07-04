using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Processings;

internal class MailSenderProcessingService(IMailSenderService service) : IMailSenderProcessingService
{
    public MailSender Get(Guid id) => service.Get(id);

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) => service.GetAll(ignoreFilters);

    public ValueTask<MailSender> AddAsync(MailSender entity) => service.AddAsync(entity);

    public ValueTask<MailSender> UpdateAsync(MailSender entity) => service.UpdateAsync(entity);

    public ValueTask<int> DeleteAsync(Guid id) => service.DeleteAsync(id);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId);

    public ValueTask DeleteAllAsync(IEnumerable<MailSender> items) => service.DeleteAllAsync(items);
}
