// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class QueuedEmailOrchestrationService(IQueuedEmailProcessingService processingService, IQueuedEmailEventProcessingService eventService)
    : IQueuedEmailOrchestrationService,
      global::cCoder.Mail.Exposures.IQueuedEmailManager
{
    public QueuedEmail GetQueuedEmail(int queuedEmailId) =>
        TryCatch<QueuedEmail>(operation: () =>
    {
        ValidateQueuedEmailOnGet(inputs: [queuedEmailId]);

        return processingService.GetQueuedEmail(iQueuedEmailId: queuedEmailId);
    });

    public IQueryable<QueuedEmail> GetAllQueuedEmails(bool ignoreFilters = false) =>
        TryCatch<IQueryable<QueuedEmail>>(operation: () =>
    {
        ValidateAllQueuedEmailsOnGet(inputs: [ignoreFilters]);

        return processingService.GetAllQueuedEmails(ignoreFilters: ignoreFilters);
    });

    public ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateQueuedEmailOnAdd(inputs: [newQueuedEmail]);

        QueuedEmail result = await processingService.AddQueuedEmailAsync(newQueuedEmail: newQueuedEmail);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask<QueuedEmail> UpdateQueuedEmailAsync(QueuedEmail updatedQueuedEmail) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateQueuedEmailOnUpdate(inputs: [updatedQueuedEmail]);

        QueuedEmail result = await processingService.UpdateQueuedEmailAsync(updatedQueuedEmail: updatedQueuedEmail);
        await eventService.RaiseQueuedEmailUpdateEventAsync(entity: result);
        return result;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int queuedEmailId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [queuedEmailId]);

        QueuedEmail entity = processingService.GetAllQueuedEmails(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == queuedEmailId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseQueuedEmailDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(iQueuedEmailId: queuedEmailId);
    }, isValueTask: true);

    public ValueTask RetryAsync(int queuedEmailId) =>
        TryCatch(operation: () =>
        {
            ValidateDeleteAsync(inputs: [queuedEmailId]);

            return processingService.RetryAsync(
                queuedEmailId: queuedEmailId);
        }, isValueTask: true);

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateByAppIdOnDelete(inputs: [appId]);

            return processingService.DeleteByAppIdAsync(appId: appId);
        }, isValueTask: true);

    public ValueTask<IEnumerable<Result<QueuedEmail>>> AddOrUpdateQueuedEmailResults(IEnumerable<QueuedEmail> newQueuedEmail) =>
        TryCatch<IEnumerable<Result<QueuedEmail>>>(operation: () =>
    {
        ValidateOrUpdateQueuedEmailResultsOnAdd(inputs: [newQueuedEmail]);

        return processingService.AddOrUpdateQueuedEmailResults(newQueuedEmail: newQueuedEmail);
    }, isValueTask: true);

    public ValueTask DeleteAllQueuedEmailAsync(IEnumerable<QueuedEmail> deletedQueuedEmail) =>
        TryCatch(operation: () =>
    {
        ValidateAllQueuedEmailOnDelete(inputs: [deletedQueuedEmail]);

        return processingService.DeleteAllQueuedEmailAsync(deletedQueuedEmail: deletedQueuedEmail);
    }, isValueTask: true);

    public ValueTask<QueuedEmail> AddQueuedEmailAsync(QueuedEmail newQueuedEmail, bool checkPrivs) =>
        TryCatch<QueuedEmail>(operation: async () =>
    {
        ValidateQueuedEmailOnAdd(inputs: [newQueuedEmail, checkPrivs]);

        QueuedEmail result = await processingService.AddQueuedEmailAsync(newQueuedEmail: newQueuedEmail, checkPrivs: checkPrivs);
        await eventService.RaiseQueuedEmailAddEventAsync(entity: result);
        return result;
    }, isValueTask: true);
}