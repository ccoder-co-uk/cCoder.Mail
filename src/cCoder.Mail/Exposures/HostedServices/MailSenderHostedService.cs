using cCoder.Mail.Services.Orchestrations;
using Microsoft.Extensions.Hosting;


namespace cCoder.Mail.Exposures.HostedServices;

public sealed class MailSenderHostedService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<MailSenderHostedService> log)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                IMailSenderOrchestrationService orchestrationService =
                    scope.ServiceProvider.GetRequiredService<IMailSenderOrchestrationService>();
                await orchestrationService.RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
            }
        }
    }
}
