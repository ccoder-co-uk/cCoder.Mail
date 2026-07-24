// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal sealed partial class ReceivedEmailOrchestrationService(
    IReceivedEmailProcessingService receivedEmailProcessingService,
    IMailReceivingProcessingService mailReceivingProcessingService)
    : IReceivedEmailOrchestrationService
{
    public ValueTask<ReceivedEmail> AddReceivedEmailAsync(
        ReceivedEmail newReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateReceivedEmailOnAdd(inputs: [newReceivedEmail]);

            return receivedEmailProcessingService.AddReceivedEmailAsync(
                newReceivedEmail: newReceivedEmail);
        }, isValueTask: true);

    public ValueTask<ReceivedEmail> UpdateReceivedEmailAsync(
        ReceivedEmail updatedReceivedEmail) =>
        TryCatch<ReceivedEmail>(operation: () =>
        {
            ValidateReceivedEmailOnUpdate(inputs: [updatedReceivedEmail]);

            return receivedEmailProcessingService.UpdateReceivedEmailAsync(
                updatedReceivedEmail: updatedReceivedEmail);
        }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateByAppIdOnDelete(inputs: [appId]);

            return receivedEmailProcessingService.DeleteByAppIdAsync(
                appId: appId);
        }, isValueTask: true);

    public Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveMailboxReceiveRequestAsync(
                inputs: [request, cancellationToken]);

            return mailReceivingProcessingService
                .ReceiveMailboxReceiveRequestAsync(
                    request: request,
                    cancellationToken: cancellationToken);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveTopAsync(inputs: [count, cancellationToken]);

            return mailReceivingProcessingService.ReceiveTopAsync(
                count: count,
                cancellationToken: cancellationToken);
        }, isTask: true);
}