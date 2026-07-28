// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Services.Orchestrations;
using Microsoft.Extensions.Hosting;

namespace cCoder.Mail.Exposures.HostedServices;

public sealed class MailReceiverHostedService(
    IMailReceiverOrchestrationService orchestrationService)
    : BackgroundService,
        IMailReceiverHostedService
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken) =>
        orchestrationService.RunContinuouslyAsync(
            cancellationToken: stoppingToken);
}