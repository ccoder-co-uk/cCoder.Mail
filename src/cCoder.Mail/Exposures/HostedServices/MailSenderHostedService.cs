// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Mail.Exposures.HostedServices;

public sealed class MailSenderHostedService(
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService,
        IMailSenderHostedService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using IServiceScope scope =
            serviceScopeFactory.CreateScope();

        IMailSenderOrchestrationService orchestrationService =
            scope.ServiceProvider.GetRequiredService<
                IMailSenderOrchestrationService>();

        await orchestrationService.RunContinuouslyAsync(
            cancellationToken: stoppingToken);
    }
}