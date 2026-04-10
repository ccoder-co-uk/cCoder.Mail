using cCoder.Data.Models.Mail;
using EventLibrary.Models;


namespace cCoder.Mail.Brokers.Events;

public interface IQueuedEmailEventBroker
{
    ValueTask RaiseQueuedEmailAddEventAsync(EventMessage<QueuedEmail> message);
    ValueTask RaiseQueuedEmailUpdateEventAsync(EventMessage<QueuedEmail> message);
    ValueTask RaiseQueuedEmailDeleteEventAsync(EventMessage<QueuedEmail> message);
}







