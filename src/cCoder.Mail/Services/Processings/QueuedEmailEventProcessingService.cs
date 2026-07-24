// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;


namespace cCoder.Mail.Services.Processings;

internal class QueuedEmailEventProcessingService(IQueuedEmailEventService eventService) : IQueuedEmailEventProcessingService
{
    public ValueTask RaiseQueuedEmailAddEventAsync(QueuedEmail entity) =>
        eventService.RaiseQueuedEmailAddEventAsync(entity: entity);

    public ValueTask RaiseQueuedEmailUpdateEventAsync(QueuedEmail entity) =>
        eventService.RaiseQueuedEmailUpdateEventAsync(entity: entity);

    public ValueTask RaiseQueuedEmailDeleteEventAsync(QueuedEmail entity) =>
        eventService.RaiseQueuedEmailDeleteEventAsync(entity: entity);
}