// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Processings;
using cCoder.Mail.Services.Orchestrations;

namespace cCoder.Mail.Services.Aggregations;

internal partial class AppAggregationService(
    IMailServerOrchestrationService mailServerOrchestrationService,
    IMailSenderProcessingService mailSenderProcessingService,
    IMailReceiverProcessingService mailReceiverProcessingService,
    IQueuedEmailOrchestrationService queuedEmailOrchestrationService,
    ISentEmailOrchestrationService sentEmailOrchestrationService,
    IReceivedEmailProcessingService receivedEmailProcessingService
) : IAppAggregationService
{
    public ValueTask AddAppAsync(App newApp) =>
        TryCatch(operation: async () =>
    {
        ValidateAppOnAdd(inputs: [newApp]);

        StampMail(app: newApp);
        _ = await mailServerOrchestrationService.AddOrUpdateMailServerResult(newMailServer: newApp.MailServers ?? []);
        await AddOrUpdateAsync(newMailSender: newApp.MailSenders ?? [], service: mailSenderProcessingService);
        await AddOrUpdateAsync(newMailReceiver: newApp.MailReceivers ?? [], service: mailReceiverProcessingService);
        _ = await queuedEmailOrchestrationService.AddOrUpdateQueuedEmailResult(newQueuedEmail: newApp.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdateSentEmailResult(newSentEmail: newApp.SentMail ?? []);
        await AddOrUpdateAsync(newReceivedEmail: newApp.ReceivedMail ?? [], service: receivedEmailProcessingService);
    }, isValueTask: true);

    public ValueTask UpdateAppAsync(App updatedApp) =>
        TryCatch(operation: async () =>
    {
        ValidateAppOnUpdate(inputs: [updatedApp]);

        StampMail(app: updatedApp);
        _ = await mailServerOrchestrationService.AddOrUpdateMailServerResult(newMailServer: updatedApp.MailServers ?? []);
        await AddOrUpdateAsync(newMailSender: updatedApp.MailSenders ?? [], service: mailSenderProcessingService);
        await AddOrUpdateAsync(newMailReceiver: updatedApp.MailReceivers ?? [], service: mailReceiverProcessingService);
        _ = await queuedEmailOrchestrationService.AddOrUpdateQueuedEmailResult(newQueuedEmail: updatedApp.MailQueue ?? []);
        _ = await sentEmailOrchestrationService.AddOrUpdateSentEmailResult(newSentEmail: updatedApp.SentMail ?? []);
        await AddOrUpdateAsync(newReceivedEmail: updatedApp.ReceivedMail ?? [], service: receivedEmailProcessingService);
    }, isValueTask: true);

    public ValueTask DeleteAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [appId]);

        await queuedEmailOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await sentEmailOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await receivedEmailProcessingService.DeleteByAppIdAsync(appId: appId);
        await mailServerOrchestrationService.DeleteByAppIdAsync(appId: appId);
        await mailSenderProcessingService.DeleteByAppIdAsync(appId: appId);
        await mailReceiverProcessingService.DeleteByAppIdAsync(appId: appId);
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
        IEnumerable<MailSender> newMailSender,
        IMailSenderProcessingService service)
    {
        foreach (MailSender item in newMailSender)
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
        IEnumerable<MailReceiver> newMailReceiver,
        IMailReceiverProcessingService service)
    {
        foreach (MailReceiver item in newMailReceiver)
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
        IEnumerable<ReceivedEmail> newReceivedEmail,
        IReceivedEmailProcessingService service)
    {
        foreach (ReceivedEmail item in newReceivedEmail)
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