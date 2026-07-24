// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailClientOrchestrationService
{
    Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveMailboxReceiveRequestAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default);

    Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default);
}