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
    public ReceivedEmail Get(int receivedEmailId) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateGet(inputs: [receivedEmailId]);

            return processingService.Get(iReceivedEmailId: receivedEmailId);
        });

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        TryCatch<IQueryable<ReceivedEmail>>(operation: () =>
        {
            ValidateGetAll(inputs: [ignoreFilters]);

            return processingService.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<ReceivedEmail> AddAsync(ReceivedEmail newReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateAddAsync(inputs: [newReceivedEmail]);

            return processingService.AddAsync(newReceivedEmail: newReceivedEmail);
        }, isValueTask: true);

    public ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail updatedReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateUpdateAsync(inputs: [updatedReceivedEmail]);

            return processingService.UpdateAsync(updatedReceivedEmail: updatedReceivedEmail);
        }, isValueTask: true);

    public ValueTask<int> DeleteAsync(int receivedEmailId) =>
        TryCatch<int>(operation: () =>
        {
            ValidateDeleteAsync(inputs: [receivedEmailId]);

            return processingService.DeleteAsync(iReceivedEmailId: receivedEmailId);
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