// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;


namespace cCoder.Mail.Services.Processings;

internal partial class QueuedEmailEventProcessingService(IQueuedEmailEventService eventService) : IQueuedEmailEventProcessingService
{
    public ValueTask RaiseQueuedEmailAddEventAsync(QueuedEmail entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseQueuedEmailAddEventAsync(inputs: [entity]);

            return eventService.RaiseQueuedEmailAddEventAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask RaiseQueuedEmailUpdateEventAsync(QueuedEmail entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseQueuedEmailUpdateEventAsync(inputs: [entity]);

            return eventService.RaiseQueuedEmailUpdateEventAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask RaiseQueuedEmailDeleteEventAsync(QueuedEmail entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseQueuedEmailDeleteEventAsync(inputs: [entity]);

            return eventService.RaiseQueuedEmailDeleteEventAsync(entity: entity);
        }, isValueTask: true);
}