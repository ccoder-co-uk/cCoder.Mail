// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Services.Orchestrations;
using Microsoft.Extensions.Hosting;


namespace cCoder.Mail.Exposures.HostedServices;

public sealed class MailSenderHostedService(
    IMailSenderOrchestrationService orchestrationService)
    : BackgroundService,
        IMailSenderHostedService
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken) =>
        orchestrationService.RunContinuouslyAsync(
            cancellationToken: stoppingToken);
}