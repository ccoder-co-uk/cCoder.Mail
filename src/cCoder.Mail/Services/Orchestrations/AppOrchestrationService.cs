// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        StampMail(app: app);
        _ = await mailServerOrchestrationService.AddOrUpdate(items: app.MailServers ?? []);
        await AddOrUpdateAsync(items: app.MailSenders ?? [], service: mailSenderConfigurationOrchestrationService);
        await AddOrUpdateAsync(items: app.MailReceivers ?? [], service: mailReceiverConfigurationOrchestrationService);
        _ = await queuedEmailOrchestrationService.AddOrUpdate(items: app.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdate(items: app.SentMail ?? []);
        await AddOrUpdateAsync(items: app.ReceivedMail ?? [], service: receivedEmailOrchestrationService);
    }

    public async ValueTask UpdateAsync(App app)
    {
        StampMail(app: app);
        _ = await mailServerOrchestrationService.AddOrUpdate(items: app.MailServers ?? []);
        await AddOrUpdateAsync(items: app.MailSenders ?? [], service: mailSenderConfigurationOrchestrationService);
        await AddOrUpdateAsync(items: app.MailReceivers ?? [], service: mailReceiverConfigurationOrchestrationService);
        _ = await queuedEmailOrchestrationService.AddOrUpdate(items: app.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdate(items: app.SentMail ?? []);
        await AddOrUpdateAsync(items: app.ReceivedMail ?? [], service: receivedEmailOrchestrationService);
    }

    public async ValueTask DeleteAsync(int appId)
    {
        await queuedEmailOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await sentEmailOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await receivedEmailOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await mailServerOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await mailSenderConfigurationOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await mailReceiverConfigurationOrchestrationService.DeleteByAppIdAsync(appId: appId);
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
                _ = await service.AddAsync(entity: item);
            else
                _ = await service.UpdateAsync(entity: item);
        }
    }

    private static async ValueTask AddOrUpdateAsync(
        IEnumerable<MailReceiver> items,
        IMailReceiverConfigurationOrchestrationService service)
    {
        foreach (MailReceiver item in items)
        {
            if (item.Id == Guid.Empty)
                _ = await service.AddAsync(entity: item);
            else
                _ = await service.UpdateAsync(entity: item);
        }
    }

    private static async ValueTask AddOrUpdateAsync(
        IEnumerable<ReceivedEmail> items,
        IReceivedEmailOrchestrationService service)
    {
        foreach (ReceivedEmail item in items)
        {
            if (item.Id == 0)
                _ = await service.AddAsync(entity: item);
            else
                _ = await service.UpdateAsync(entity: item);
        }
    }

}