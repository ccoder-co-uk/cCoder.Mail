using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Orchestrations;

internal class AppOrchestrationService(
    IMailServerOrchestrationService mailServerOrchestrationService,
    IQueuedEmailOrchestrationService queuedEmailOrchestrationService,
    ISentEmailOrchestrationService sentEmailOrchestrationService
) : IAppOrchestrationService
{
    public async ValueTask AddAsync(App app)
    {
        StampMail(app);
        _ = await mailServerOrchestrationService.AddOrUpdate(app.MailServers ?? []);
        _ = await queuedEmailOrchestrationService.AddOrUpdate(app.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdate(app.SentMail ?? []);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampMail(app);
        _ = await mailServerOrchestrationService.AddOrUpdate(app.MailServers ?? []);
        _ = await queuedEmailOrchestrationService.AddOrUpdate(app.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdate(app.SentMail ?? []);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        await mailServerOrchestrationService.DeleteByAppIdAsync(appId);
        await queuedEmailOrchestrationService.DeleteByAppIdAsync(appId);
        await sentEmailOrchestrationService.DeleteByAppIdAsync(appId);
    }

    private static void StampMail(App app)
    {
        foreach (MailServer mailServer in app.MailServers ?? [])
            mailServer.AppId = app.Id;

        foreach (QueuedEmail queuedEmail in app.MailQueue ?? [])
            queuedEmail.AppId = app.Id;

        foreach (SentEmail sentEmail in app.SentMail ?? [])
            sentEmail.AppId = app.Id;
    }
}

