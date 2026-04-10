using cCoder.Data.Models.Mail;
using EventLibrary;
using EventLibrary.Models;


namespace cCoder.Mail.Brokers.Events;

public class MailServerEventBroker(IEventHub eventHub) : IMailServerEventBroker
{
    public ValueTask RaiseMailServerAddEventAsync(EventMessage<MailServer> message) =>
        eventHub.RaiseEventAsync("mail_server_add", message);

    public ValueTask RaiseMailServerUpdateEventAsync(EventMessage<MailServer> message) =>
        eventHub.RaiseEventAsync("mail_server_update", message);

    public ValueTask RaiseMailServerDeleteEventAsync(EventMessage<MailServer> message) =>
        eventHub.RaiseEventAsync("mail_server_delete", message);
}







