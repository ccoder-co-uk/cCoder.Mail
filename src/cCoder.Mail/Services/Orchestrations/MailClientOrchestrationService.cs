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
    public Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            ValidateSendAsync(inputs: [email, cancellationToken]);

            return mailSendingService.SendAsync(email: email, cancellationToken: cancellationToken);
        }, isTask: true);

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: () =>
        {
            ValidateReceiveAsync(inputs: [request, cancellationToken]);

            return mailReceivingService.ReceiveAsync(request: request, cancellationToken: cancellationToken);
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