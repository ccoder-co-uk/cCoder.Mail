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
    public ValueTask RaiseMailServerAddEventAsync(MailServer mailServer) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseMailServerAddEventAsync(inputs: [mailServer]);

            return eventService.RaiseMailServerAddEventAsync(entity: mailServer);
        }, isValueTask: true);

    public ValueTask RaiseMailServerUpdateEventAsync(MailServer mailServer) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseMailServerUpdateEventAsync(inputs: [mailServer]);

            return eventService.RaiseMailServerUpdateEventAsync(entity: mailServer);
        }, isValueTask: true);

    public ValueTask RaiseMailServerDeleteEventAsync(MailServer mailServer) =>
        TryCatch(operation: () =>
        {
            ValidateRaiseMailServerDeleteEventAsync(inputs: [mailServer]);

            return eventService.RaiseMailServerDeleteEventAsync(entity: mailServer);
        }, isValueTask: true);
}