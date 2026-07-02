using cCoder.Mail.Exposures.HostedServices;
using cCoder.Mail.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;


namespace cCoder.Mail.Tests;

public class HostedServicesRegistrationTests
{
    [Fact]
    public void AddMailHostedServices_RegistersMailSenderHostedService()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddMailHostedServices();

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationFactory is not null);
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IMailSenderHostedService)
                && descriptor.ImplementationType == typeof(MailSenderHostedService));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IMailReceiverHostedService)
                && descriptor.ImplementationType == typeof(MailReceiverHostedService));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IMailSenderOrchestrationService)
                && descriptor.ImplementationType?.Name == "MailSenderOrchestrationService");
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IMailReceiverOrchestrationService)
                && descriptor.ImplementationType?.Name == "MailReceiverOrchestrationService");
    }
}
