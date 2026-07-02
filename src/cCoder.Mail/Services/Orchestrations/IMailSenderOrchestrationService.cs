namespace cCoder.Mail.Services.Orchestrations;

public interface IMailSenderOrchestrationService
{
    Task RunAsync(CancellationToken cancellationToken = default);

    Task RunContinuouslyAsync(CancellationToken cancellationToken = default);
}
