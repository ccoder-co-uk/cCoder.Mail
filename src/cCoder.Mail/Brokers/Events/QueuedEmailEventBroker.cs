using cCoder.Data.Models.Mail;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.Mail.Brokers.Events;

public class QueuedEmailEventBroker(IEventHub eventHub) : IQueuedEmailEventBroker
{
    public ValueTask RaiseQueuedEmailAddEventAsync(EventMessage<QueuedEmail> message) =>
        eventHub.RaiseEventAsync("queued_email_add", message);

    public ValueTask RaiseQueuedEmailUpdateEventAsync(EventMessage<QueuedEmail> message) =>
        eventHub.RaiseEventAsync("queued_email_update", message);

    public ValueTask RaiseQueuedEmailDeleteEventAsync(EventMessage<QueuedEmail> message) =>
        eventHub.RaiseEventAsync("queued_email_delete", message);
}







