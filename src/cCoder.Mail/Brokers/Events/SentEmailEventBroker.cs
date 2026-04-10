using cCoder.Data.Models.Mail;
using EventLibrary;
using EventLibrary.Models;


namespace cCoder.Mail.Brokers.Events;

public class SentEmailEventBroker(IEventHub eventHub) : ISentEmailEventBroker
{
    public ValueTask RaiseSentEmailAddEventAsync(EventMessage<SentEmail> message) =>
        eventHub.RaiseEventAsync("sent_email_add", message);

    public ValueTask RaiseSentEmailUpdateEventAsync(EventMessage<SentEmail> message) =>
        eventHub.RaiseEventAsync("sent_email_update", message);

    public ValueTask RaiseSentEmailDeleteEventAsync(EventMessage<SentEmail> message) =>
        eventHub.RaiseEventAsync("sent_email_delete", message);
}







