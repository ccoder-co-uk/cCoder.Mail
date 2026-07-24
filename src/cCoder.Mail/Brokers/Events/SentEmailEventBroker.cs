// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Mail.Brokers.Events;

public class SentEmailEventBroker(IEventHub eventHub) : ISentEmailEventBroker
{
    public ValueTask RaiseSentEmailAddEventAsync(EventMessage<SentEmail> message) =>
        eventHub.RaiseEventAsync(name: "sent_email_add", message: message);

    public ValueTask RaiseSentEmailUpdateEventAsync(EventMessage<SentEmail> message) =>
        eventHub.RaiseEventAsync(name: "sent_email_update", message: message);

    public ValueTask RaiseSentEmailDeleteEventAsync(EventMessage<SentEmail> message) =>
        eventHub.RaiseEventAsync(name: "sent_email_delete", message: message);
}