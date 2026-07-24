// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;


namespace cCoder.Mail.Services.Processings;

internal class SentEmailEventProcessingService(ISentEmailEventService eventService) : ISentEmailEventProcessingService
{
    public ValueTask RaiseSentEmailAddEventAsync(SentEmail entity) =>
        eventService.RaiseSentEmailAddEventAsync(entity: entity);

    public ValueTask RaiseSentEmailUpdateEventAsync(SentEmail entity) =>
        eventService.RaiseSentEmailUpdateEventAsync(entity: entity);

    public ValueTask RaiseSentEmailDeleteEventAsync(SentEmail entity) =>
        eventService.RaiseSentEmailDeleteEventAsync(entity: entity);
}