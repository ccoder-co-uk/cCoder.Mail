using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Foundations.Events;

public interface IQueuedEmailEventService
{
    ValueTask RaiseQueuedEmailAddEventAsync(QueuedEmail entity);
    ValueTask RaiseQueuedEmailUpdateEventAsync(QueuedEmail entity);
    ValueTask RaiseQueuedEmailDeleteEventAsync(QueuedEmail entity);
}









