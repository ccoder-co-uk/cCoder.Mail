// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Services.Orchestrations;
using Microsoft.Extensions.Hosting;

namespace cCoder.Mail.Exposures.HostedServices;

public sealed class MailReceiverHostedService(
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService,
        IMailReceiverHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        IMailReceiverOrchestrationService mailReceiverOrchestrationService =
            scope.ServiceProvider.GetRequiredService<IMailReceiverOrchestrationService>();

        await mailReceiverOrchestrationService.RunContinuouslyAsync(cancellationToken: stoppingToken);
    }
}