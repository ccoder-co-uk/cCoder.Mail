// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Orchestrations;

internal partial class AppOrchestrationService(
    IMailServerOrchestrationService mailServerOrchestrationService,
    IMailSenderConfigurationOrchestrationService mailSenderConfigurationOrchestrationService,
    IMailReceiverConfigurationOrchestrationService mailReceiverConfigurationOrchestrationService,
    IQueuedEmailOrchestrationService queuedEmailOrchestrationService,
    ISentEmailOrchestrationService sentEmailOrchestrationService,
    IReceivedEmailOrchestrationService receivedEmailOrchestrationService
) : IAppOrchestrationService
{
    public ValueTask AddAppAsync(App newApp) =>
        TryCatch(operation: async () =>
    {
        ValidateAddAsync(inputs: [newApp]);

        StampMail(app: newApp);
        _ = await mailServerOrchestrationService.AddOrUpdateMailServerResult(items: newApp.MailServers ?? []);
        await AddOrUpdateAsync(items: newApp.MailSenders ?? [], service: mailSenderConfigurationOrchestrationService);
        await AddOrUpdateAsync(items: newApp.MailReceivers ?? [], service: mailReceiverConfigurationOrchestrationService);
        _ = await queuedEmailOrchestrationService.AddOrUpdateQueuedEmailResult(items: newApp.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdateSentEmailResult(items: newApp.SentMail ?? []);
        await AddOrUpdateAsync(items: newApp.ReceivedMail ?? [], service: receivedEmailOrchestrationService);
    }, isValueTask: true);

    public ValueTask UpdateAppAsync(App updatedApp) =>
        TryCatch(operation: async () =>
    {
        ValidateUpdateAsync(inputs: [updatedApp]);

        StampMail(app: updatedApp);
        _ = await mailServerOrchestrationService.AddOrUpdateMailServerResult(items: updatedApp.MailServers ?? []);
        await AddOrUpdateAsync(items: updatedApp.MailSenders ?? [], service: mailSenderConfigurationOrchestrationService);
        await AddOrUpdateAsync(items: updatedApp.MailReceivers ?? [], service: mailReceiverConfigurationOrchestrationService);
        _ = await queuedEmailOrchestrationService.AddOrUpdateQueuedEmailResult(items: updatedApp.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdateSentEmailResult(items: updatedApp.SentMail ?? []);
        await AddOrUpdateAsync(items: updatedApp.ReceivedMail ?? [], service: receivedEmailOrchestrationService);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [appId]);

        await queuedEmailOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await sentEmailOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await receivedEmailOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await mailServerOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await mailSenderConfigurationOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await mailReceiverConfigurationOrchestrationService.DeleteByAppIdAsync(appId: appId);
    }, isValueTask: true);

    private static void StampMail(App app)
    {
        foreach (MailServer mailServer in app.MailServers ?? [])
        {
            mailServer.AppId = app.Id;
        }

        foreach (MailSender mailSender in app.MailSenders ?? [])
        {
            mailSender.AppId = app.Id;
        }

        foreach (MailReceiver mailReceiver in app.MailReceivers ?? [])
        {
            mailReceiver.AppId = app.Id;
        }

        foreach (QueuedEmail queuedEmail in app.MailQueue ?? [])
        {
            queuedEmail.AppId = app.Id;
        }

        foreach (SentEmail sentEmail in app.SentMail ?? [])
        {
            sentEmail.AppId = app.Id;
        }

        foreach (ReceivedEmail receivedEmail in app.ReceivedMail ?? [])
        {
            receivedEmail.AppId = app.Id;
        }
    }

    private static async ValueTask AddOrUpdateAsync(
        IEnumerable<MailSender> items,
        IMailSenderConfigurationOrchestrationService service)
    {
        foreach (MailSender item in items)
        {
            if (item.Id == Guid.Empty)
            {
                _ = await service.AddMailSenderAsync(newMailSender: item);
            }
            else
            {
                _ = await service.UpdateMailSenderAsync(updatedMailSender: item);
            }
        }
    }

    private static async ValueTask AddOrUpdateAsync(
        IEnumerable<MailReceiver> items,
        IMailReceiverConfigurationOrchestrationService service)
    {
        foreach (MailReceiver item in items)
        {
            if (item.Id == Guid.Empty)
            {
                _ = await service.AddMailReceiverAsync(newMailReceiver: item);
            }
            else
            {
                _ = await service.UpdateMailReceiverAsync(updatedMailReceiver: item);
            }
        }
    }

    private static async ValueTask AddOrUpdateAsync(
        IEnumerable<ReceivedEmail> items,
        IReceivedEmailOrchestrationService service)
    {
        foreach (ReceivedEmail item in items)
        {
            if (item.Id == 0)
            {
                _ = await service.AddReceivedEmailAsync(newReceivedEmail: item);
            }
            else
            {
                _ = await service.UpdateReceivedEmailAsync(updatedReceivedEmail: item);
            }
        }
    }

}