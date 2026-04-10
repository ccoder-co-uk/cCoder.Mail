using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;


namespace cCoder.Mail.Services.Processings;

internal class MailServerEventProcessingService(IMailServerEventService eventService) : IMailServerEventProcessingService
{
    public ValueTask RaiseMailServerAddEventAsync(MailServer entity) => eventService.RaiseMailServerAddEventAsync(entity);

    public ValueTask RaiseMailServerUpdateEventAsync(MailServer entity) => eventService.RaiseMailServerUpdateEventAsync(entity);

    public ValueTask RaiseMailServerDeleteEventAsync(MailServer entity) => eventService.RaiseMailServerDeleteEventAsync(entity);
}








