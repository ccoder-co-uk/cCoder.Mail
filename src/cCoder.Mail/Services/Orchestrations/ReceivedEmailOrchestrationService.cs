// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal class ReceivedEmailOrchestrationService(
    IReceivedEmailProcessingService processingService,
    IMailReceivingService mailReceivingService)
    : IReceivedEmailOrchestrationService
{
    public ReceivedEmail Get(int id) =>
        processingService.Get(id: id);

    public IQueryable<ReceivedEmail> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<ReceivedEmail> AddAsync(ReceivedEmail entity) =>
        processingService.AddAsync(entity: entity);

    public ValueTask<ReceivedEmail> UpdateAsync(ReceivedEmail entity) =>
        processingService.UpdateAsync(entity: entity);

    public ValueTask<int> DeleteAsync(int id) =>
        processingService.DeleteAsync(id: id);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        mailReceivingService.ReceiveAsync(request: request, cancellationToken: cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        mailReceivingService.ReceiveTopAsync(count: count, cancellationToken: cancellationToken);
}