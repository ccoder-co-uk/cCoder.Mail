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
            ValidateGet(inputs: [mailReceiverConfigurationId]);

            return processingService.GetMailReceiver(iMailReceiverId: mailReceiverConfigurationId);
        });

    public IQueryable<MailReceiver> GetAllMailReceiver(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailReceiver>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return processingService.GetAllMailReceiver(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateAddAsync(inputs: [newMailReceiver]);

            return processingService.AddMailReceiverAsync(newMailReceiver: newMailReceiver);
        }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [updatedMailReceiver]);

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
            ValidateDeleteByAppIdAsync(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);
}