// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;


namespace cCoder.Mail.Services.Processings;

internal partial class MailServerEventProcessingService(IMailServerEventService eventService) : IMailServerEventProcessingService
{
    public ValueTask RaiseMailServerAddEventAsync(MailServer entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseMailServerAddEventAsync(inputs: [entity]);

            return eventService.RaiseMailServerAddEventAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask RaiseMailServerUpdateEventAsync(MailServer entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseMailServerUpdateEventAsync(inputs: [entity]);

            return eventService.RaiseMailServerUpdateEventAsync(entity: entity);
        }, isValueTask: true);

    public ValueTask RaiseMailServerDeleteEventAsync(MailServer entity) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseMailServerDeleteEventAsync(inputs: [entity]);

            return eventService.RaiseMailServerDeleteEventAsync(entity: entity);
        }, isValueTask: true);
}