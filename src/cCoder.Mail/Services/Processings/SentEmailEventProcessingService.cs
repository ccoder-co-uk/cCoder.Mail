// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;


namespace cCoder.Mail.Services.Processings;

internal partial class SentEmailEventProcessingService(ISentEmailEventService eventService) : ISentEmailEventProcessingService
{
    public ValueTask RaiseSentEmailAddEventAsync(SentEmail entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseSentEmailAddEventAsync(inputs: [entity]);

            return eventService.RaiseSentEmailAddEventAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask RaiseSentEmailUpdateEventAsync(SentEmail entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseSentEmailUpdateEventAsync(inputs: [entity]);

            return eventService.RaiseSentEmailUpdateEventAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask RaiseSentEmailDeleteEventAsync(SentEmail entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseSentEmailDeleteEventAsync(inputs: [entity]);

            return eventService.RaiseSentEmailDeleteEventAsync(entity: entity);
        }, isValueTask: true);
}