// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class MailReceiverConfigurationOrchestrationService(IMailReceiverProcessingService processingService)
    : IMailReceiverConfigurationOrchestrationService
{
    public MailReceiver GetMailReceiver(Guid mailReceiverConfigurationId) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateMailReceiverOnGet(inputs: [mailReceiverConfigurationId]);

            return processingService.GetMailReceiver(iMailReceiverId: mailReceiverConfigurationId);
        });

    public IQueryable<MailReceiver> GetAllMailReceiver(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailReceiver>>(operation: () =>
        {
            ValidateAllMailReceiverOnGet(inputs: [ignoreFilters]);

            return processingService.GetAllMailReceiver(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateMailReceiverOnAdd(inputs: [newMailReceiver]);

            return processingService.AddMailReceiverAsync(newMailReceiver: newMailReceiver);
        }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateMailReceiverOnUpdate(inputs: [updatedMailReceiver]);

            return processingService.UpdateMailReceiverAsync(updatedMailReceiver: updatedMailReceiver);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(Guid mailReceiverConfigurationId) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [mailReceiverConfigurationId]);

            return processingService.DeleteAsync(iMailReceiverId: mailReceiverConfigurationId);
        }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateByAppIdOnDelete(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);
}