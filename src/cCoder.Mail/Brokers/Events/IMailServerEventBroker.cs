using cCoder.Data.Models.Mail;
using cCoder.Eventing.Models;


namespace cCoder.Mail.Brokers.Events;

public interface IMailServerEventBroker
{
    ValueTask RaiseMailServerAddEventAsync(EventMessage<MailServer> message);
    ValueTask RaiseMailServerUpdateEventAsync(EventMessage<MailServer> message);
    ValueTask RaiseMailServerDeleteEventAsync(EventMessage<MailServer> message);
}







