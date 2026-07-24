// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class MailSenderConfigurationOrchestrationService(IMailSenderProcessingService processingService)
    : IMailSenderConfigurationOrchestrationService
{
    public MailSender GetMailSender(Guid mailSenderConfigurationId) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateMailSenderOnGet(inputs: [mailSenderConfigurationId]);

            return processingService.GetMailSender(iMailSenderId: mailSenderConfigurationId);
        });

    public IQueryable<MailSender> GetAllMailSender(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailSender>>(operation: () =>
        {
            ValidateAllMailSenderOnGet(inputs: [ignoreFilters]);

            return processingService.GetAllMailSender(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailSender> AddMailSenderAsync(MailSender newMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateMailSenderOnAdd(inputs: [newMailSender]);

            return processingService.AddMailSenderAsync(newMailSender: newMailSender);
        }, isValueTask: true);

    public ValueTask<MailSender> UpdateMailSenderAsync(MailSender updatedMailSender) =>
        TryCatch<MailSender>(operation: () =>
        {
            ValidateMailSenderOnUpdate(inputs: [updatedMailSender]);

            return processingService.UpdateMailSenderAsync(updatedMailSender: updatedMailSender);
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
            ValidateByAppIdOnDelete(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);
}