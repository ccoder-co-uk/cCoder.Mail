// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using Mail.HostedServices.Models;

namespace Mail.HostedServices;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMailHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MailHostedServicesConfiguration> configure = null)
    {
        MailHostedServicesConfiguration hostedConfiguration = new();
        configuration.Bind(instance: hostedConfiguration);
        configure?.Invoke(obj: hostedConfiguration);

        services.AddEventingHostedServices(
            configuration: hostedConfiguration.Eventing);
        services.AddData(configuration: hostedConfiguration.Data);
        cCoder.Mail.IServiceCollectionExtensions
            .AddMailHostedServices(
                services: services,
                configuration: hostedConfiguration.Mail);

        return services;
    }
}