// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Services.Foundations.Events;

namespace cCoder.Mail.Exposures.EventHandlers;

internal class MailEventHandlers(IEventHandlerService eventHandlerService) : IMailEventHandlers
{
    public void ListenToAllEvents() =>
        eventHandlerService.ListenToAllEvents();
}