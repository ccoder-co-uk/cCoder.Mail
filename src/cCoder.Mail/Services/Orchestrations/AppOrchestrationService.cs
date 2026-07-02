using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Orchestrations;

internal class AppOrchestrationService(
    IMailServerOrchestrationService mailServerOrchestrationService,
    IMailSenderConfigurationOrchestrationService mailSenderConfigurationOrchestrationService,
    IMailReceiverConfigurationOrchestrationService mailReceiverConfigurationOrchestrationService,
    IQueuedEmailOrchestrationService queuedEmailOrchestrationService,
    ISentEmailOrchestrationService sentEmailOrchestrationService,
    IReceivedEmailOrchestrationService receivedEmailOrchestrationService
) : IAppOrchestrationService
{
    public async ValueTask AddAsync(App app)
    {
        StampMail(app);
        _ = await mailServerOrchestrationService.AddOrUpdate(app.MailServers ?? []);
        await AddOrUpdateAsync(app.MailSenders ?? [], mailSenderConfigurationOrchestrationService);
        await AddOrUpdateAsync(app.MailReceivers ?? [], mailReceiverConfigurationOrchestrationService);
        _ = await queuedEmailOrchestrationService.AddOrUpdate(app.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdate(app.SentMail ?? []);
        await AddOrUpdateAsync(app.ReceivedMail ?? [], receivedEmailOrchestrationService);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampMail(app);
        _ = await mailServerOrchestrationService.AddOrUpdate(app.MailServers ?? []);
        await AddOrUpdateAsync(app.MailSenders ?? [], mailSenderConfigurationOrchestrationService);
        await AddOrUpdateAsync(app.MailReceivers ?? [], mailReceiverConfigurationOrchestrationService);
        _ = await queuedEmailOrchestrationService.AddOrUpdate(app.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdate(app.SentMail ?? []);
        await AddOrUpdateAsync(app.ReceivedMail ?? [], receivedEmailOrchestrationService);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        await mailServerOrchestrationService.DeleteByAppIdAsync(appId);
        await DeleteAllAsync(mailSenderConfigurationOrchestrationService.GetAll(true).Where(item => item.AppId == appId), mailSenderConfigurationOrchestrationService);
        await DeleteAllAsync(mailReceiverConfigurationOrchestrationService.GetAll(true).Where(item => item.AppId == appId), mailReceiverConfigurationOrchestrationService);
        await queuedEmailOrchestrationService.DeleteByAppIdAsync(appId);
        await sentEmailOrchestrationService.DeleteByAppIdAsync(appId);
        await DeleteAllAsync(receivedEmailOrchestrationService.GetAll(true).Where(item => item.AppId == appId), receivedEmailOrchestrationService);
    }

    private static void StampMail(App app)
    {
        foreach (MailServer mailServer in app.MailServers ?? [])
            mailServer.AppId = app.Id;

        foreach (MailSender mailSender in app.MailSenders ?? [])
            mailSender.AppId = app.Id;

        foreach (MailReceiver mailReceiver in app.MailReceivers ?? [])
            mailReceiver.AppId = app.Id;

        foreach (QueuedEmail queuedEmail in app.MailQueue ?? [])
            queuedEmail.AppId = app.Id;

        foreach (SentEmail sentEmail in app.SentMail ?? [])
            sentEmail.AppId = app.Id;

        foreach (ReceivedEmail receivedEmail in app.ReceivedMail ?? [])
            receivedEmail.AppId = app.Id;
    }

    private static async ValueTask AddOrUpdateAsync(
        IEnumerable<MailSender> items,
        IMailSenderConfigurationOrchestrationService service)
    {
        foreach (MailSender item in items)
        {
            if (item.Id == Guid.Empty)
                _ = await service.AddAsync(item);
            else
                _ = await service.UpdateAsync(item);
        }
    }

    private static async ValueTask AddOrUpdateAsync(
        IEnumerable<MailReceiver> items,
        IMailReceiverConfigurationOrchestrationService service)
    {
        foreach (MailReceiver item in items)
        {
            if (item.Id == Guid.Empty)
                _ = await service.AddAsync(item);
            else
                _ = await service.UpdateAsync(item);
        }
    }

    private static async ValueTask AddOrUpdateAsync(
        IEnumerable<ReceivedEmail> items,
        IReceivedEmailOrchestrationService service)
    {
        foreach (ReceivedEmail item in items)
        {
            if (item.Id == 0)
                _ = await service.AddAsync(item);
            else
                _ = await service.UpdateAsync(item);
        }
    }

    private static async ValueTask DeleteAllAsync(
        IEnumerable<MailSender> items,
        IMailSenderConfigurationOrchestrationService service)
    {
        foreach (MailSender item in items)
            await service.DeleteAsync(item.Id);
    }

    private static async ValueTask DeleteAllAsync(
        IEnumerable<MailReceiver> items,
        IMailReceiverConfigurationOrchestrationService service)
    {
        foreach (MailReceiver item in items)
            await service.DeleteAsync(item.Id);
    }

    private static async ValueTask DeleteAllAsync(
        IEnumerable<ReceivedEmail> items,
        IReceivedEmailOrchestrationService service)
    {
        foreach (ReceivedEmail item in items)
            await service.DeleteAsync(item.Id);
    }
}

