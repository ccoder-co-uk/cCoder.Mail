using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Orchestrations;

namespace cCoder.Mail.Exposures;

internal class MailManagerExposure(
    IQueuedEmailOrchestrationService queuedEmailOrchestrationService) : IMailManagerExposure
{
    public ValueTask<QueuedEmail> AddAsync(QueuedEmail email, bool checkPrivileges = false) =>
        queuedEmailOrchestrationService.AddAsync(email, checkPrivileges);
}

