// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Services.Orchestrations;

public interface IMailSenderOrchestrationService
{
    Task RunAsync(CancellationToken cancellationToken = default);

    Task RunContinuouslyAsync(CancellationToken cancellationToken = default);
}