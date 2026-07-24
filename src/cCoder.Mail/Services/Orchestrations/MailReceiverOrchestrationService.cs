// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Processings;

namespace cCoder.Mail.Services.Orchestrations;

internal sealed partial class MailReceiverOrchestrationService(
    IMailReceiverProcessingService mailReceiverProcessingService,
    IReceivedEmailProcessingService receivedEmailProcessingService,
    IMailReceivingService mailReceivingService,
    MailConfiguration mailConfiguration,
    ILogger<MailReceiverOrchestrationService> log)
    : IMailReceiverOrchestrationService
{
    public Task RunContinuouslyAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
    {

        ValidateRunContinuouslyAsync(inputs: [cancellationToken]);

        if (mailConfiguration.IsMigrating)
        {
            return;
        }

        using PeriodicTimer timer = new(period: TimeSpan.FromMinutes(minutes: 1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            try
            {
                await RunAsync(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                log.LogError(exception: ex, message: ex.Message);
            }
        }
    }, isTask: true);

    public Task RunAsync(CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
    {
        ValidateRunAsync(inputs: [cancellationToken]);

        MailReceiver[] receivers = mailReceiverProcessingService.GetEnabled();

        foreach (MailReceiver receiver in receivers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ReceiveAsync(receiver: receiver, cancellationToken: cancellationToken);
        }
    }, isTask: true);

    private async Task ReceiveAsync(MailReceiver receiver, CancellationToken cancellationToken)
    {
        DateTimeOffset from = receiver.LastReceivedOn ?? DateTimeOffset.UtcNow.AddDays(days: -1);
        DateTimeOffset to = DateTimeOffset.UtcNow;

        ReceivedEmail[] receivedEmails = await mailReceivingService.ReceiveAsync(
request: new MailboxReceiveRequest
{
    ProviderName = receiver.ProviderName,
    AppId = receiver.AppId,
    MailReceiverId = receiver.Id,
    Host = receiver.Host,
    Port = receiver.Port,
    EnableSSL = receiver.EnableSSL,
    User = receiver.User,
    Password = receiver.Password,
    From = from,
    To = to,
    MaximumMessages = 100,
},
cancellationToken: cancellationToken);

        ReceivedEmail[] newEmails =
        [
            .. receivedEmails
                .Where(predicate: email => !receivedEmailProcessingService.Exists(mailReceiverId: receiver.Id, messageId: email.MessageId))
            .Select(selector: email => Prepare(email: email, receiver: receiver))
        ];

        await receivedEmailProcessingService.AddRangeAsync(entities: newEmails, cancellationToken: cancellationToken);

        receiver.LastReceivedOn = to;
        await mailReceiverProcessingService.UpdateAsync(updatedMailReceiver: receiver);
    }

    private static ReceivedEmail Prepare(ReceivedEmail email, MailReceiver receiver)
    {
        email.AppId = receiver.AppId;
        email.MailReceiverId = receiver.Id;
        email.SentByUserId ??= "Guest";
        email.ReceivedOn = email.ReceivedOn == default ? DateTimeOffset.UtcNow : email.ReceivedOn;
        return email;
    }
}