using cCoder.Data.Models.Mail;
using cCoder.Eventing;
using cCoder.Eventing.Models;


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







