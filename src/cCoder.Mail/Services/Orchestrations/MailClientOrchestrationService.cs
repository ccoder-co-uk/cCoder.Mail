// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Services.Orchestrations;

internal sealed partial class MailClientOrchestrationService(
    IMailSendingService mailSendingService,
    IMailReceivingService mailReceivingService)
    : IMailClientOrchestrationService
{
    public Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateSendQueuedEmailAsync(inputs: [email, cancellationToken]);

            return mailSendingService.SendQueuedEmailAsync(email: email, cancellationToken: cancellationToken);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveMailboxReceiveRequestAsync(inputs: [request, cancellationToken]);

            return mailReceivingService.ReceiveMailboxReceiveRequestAsync(request: request, cancellationToken: cancellationToken);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveTopAsync(inputs: [count, cancellationToken]);

            return mailReceivingService.ReceiveTopAsync(count: count, cancellationToken: cancellationToken);
        }, isTask: true);
}