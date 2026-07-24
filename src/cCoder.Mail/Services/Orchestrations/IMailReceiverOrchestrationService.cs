// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailReceiverOrchestrationService
{
    ValueTask<MailReceiver> AddMailReceiverAsync(MailReceiver newMailReceiver);

    ValueTask<MailReceiver> UpdateMailReceiverAsync(MailReceiver updatedMailReceiver);

    ValueTask DeleteByAppIdAsync(int appId);

    Task RunContinuouslyAsync(CancellationToken cancellationToken = default);
    Task RunAsync(CancellationToken cancellationToken = default);
}