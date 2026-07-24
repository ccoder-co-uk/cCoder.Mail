// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class MailReceiverConfigurationOrchestrationService(IMailReceiverProcessingService processingService)
    : IMailReceiverConfigurationOrchestrationService
{
    public MailReceiver Get(Guid id) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateGet(inputs: [id]);

            return processingService.Get(id: id);
        });

    public IQueryable<MailReceiver> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<MailReceiver>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<MailReceiver> AddAsync(MailReceiver entity) =>
        TryCatch<MailReceiver>(operation: () =>
        {
            ValidateAddAsync(inputs: [entity]);

            return processingService.AddAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<MailReceiver> UpdateAsync(MailReceiver entity) =>
        TryCatch<MailReceiver>(operation: () =>
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