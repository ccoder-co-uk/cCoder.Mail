// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class ReceivedEmailOrchestrationService(
    IReceivedEmailProcessingService processingService,
    IMailReceivingService mailReceivingService)
    : IReceivedEmailOrchestrationService
{
    public ReceivedEmail Get(int id) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateGet(inputs: [id]);

            return processingService.Get(id: id);
        });

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<ReceivedEmail>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateAddAsync(inputs: [entity]);

            return processingService.AddAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [entity]);

            return processingService.UpdateAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(int id) =>
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

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveAsync(inputs: [request, cancellationToken]);

            return mailReceivingService.ReceiveAsync(request: request, cancellationToken: cancellationToken);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveTopAsync(inputs: [count, cancellationToken]);

            return mailReceivingService.ReceiveTopAsync(count: count, cancellationToken: cancellationToken);
        }, isTask: true);
}