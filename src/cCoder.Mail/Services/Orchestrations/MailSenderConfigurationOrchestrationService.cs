// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class MailSenderConfigurationOrchestrationService(IMailSenderProcessingService processingService)
    : IMailSenderConfigurationOrchestrationService
{
    public MailSender Get(Guid mailSenderConfigurationId) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateGet(inputs: [mailSenderConfigurationId]);

            return processingService.Get(iMailSenderId: mailSenderConfigurationId);
        });

    public IQueryable<MailSender> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailSender>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailSender> AddAsync(MailSender newMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateAddAsync(inputs: [newMailSender]);

            return processingService.AddAsync(newMailSender: newMailSender);
        }, isValueTask: true);

    public ValueTask<MailSender> UpdateAsync(MailSender updatedMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [updatedMailSender]);

            return processingService.UpdateAsync(updatedMailSender: updatedMailSender);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid mailSenderConfigurationId) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [mailSenderConfigurationId]);

            return processingService.DeleteAsync(iMailSenderId: mailSenderConfigurationId);
        }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);
}