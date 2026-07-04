using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal class MailSenderConfigurationOrchestrationService(IMailSenderProcessingService processingService)
    : IMailSenderConfigurationOrchestrationService
{
    public MailSender Get(Guid id) => processingService.Get(id);

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) => processingService.GetAll(ignoreFilters);

    public ValueTask<MailSender> AddAsync(MailSender entity) => processingService.AddAsync(entity);

    public ValueTask<MailSender> UpdateAsync(MailSender entity) => processingService.UpdateAsync(entity);

    public ValueTask<int> DeleteAsync(Guid id) => processingService.DeleteAsync(id);

    public ValueTask DeleteByAppIdAsync(int appId) => processingService.DeleteByAppIdAsync(appId);
}
