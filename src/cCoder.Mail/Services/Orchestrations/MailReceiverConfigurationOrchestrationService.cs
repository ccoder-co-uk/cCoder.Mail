// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal class MailReceiverConfigurationOrchestrationService(IMailReceiverProcessingService processingService)
    : IMailReceiverConfigurationOrchestrationService
{
    public MailReceiver Get(Guid id) =>
        processingService.Get(id: id);

    public IQueryable<MailReceiver> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<MailReceiver> AddAsync(MailReceiver entity) =>
        processingService.AddAsync(entity: entity);

    public ValueTask<MailReceiver> UpdateAsync(MailReceiver entity) =>
        processingService.UpdateAsync(entity: entity);

    public ValueTask<int> DeleteAsync(Guid id) =>
        processingService.DeleteAsync(id: id);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);
}