using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations.Events;

public interface IMailServerEventService
{
    ValueTask RaiseMailServerAddEventAsync(MailServer entity);
    ValueTask RaiseMailServerUpdateEventAsync(MailServer entity);
    ValueTask RaiseMailServerDeleteEventAsync(MailServer entity);
}









