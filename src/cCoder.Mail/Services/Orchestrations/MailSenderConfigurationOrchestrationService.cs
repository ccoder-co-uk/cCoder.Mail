// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class MailSenderConfigurationOrchestrationService(IMailSenderProcessingService processingService)
    : IMailSenderConfigurationOrchestrationService
{
    public MailSender Get(Guid id) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateGet(inputs: [id]);

            return processingService.Get(id: id);
        });

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailSender>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailSender> AddAsync(MailSender entity) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateAddAsync(inputs: [entity]);

            return processingService.AddAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<MailSender> UpdateAsync(MailSender entity) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [entity]);

            return processingService.UpdateAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid id) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [id]);

            return processingService.DeleteAsync(id: id);
        }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);
}