// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Security;
using Mail.Web.Models;

namespace Mail.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMailWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MailWebConfiguration> configure = null)
    {
        MailWebConfiguration webConfiguration = new();
        configuration.Bind(instance: webConfiguration);
        configure?.Invoke(obj: webConfiguration);

        services.AddEventingWeb(configuration: webConfiguration.Eventing);
        services.AddData(configuration: webConfiguration.Data);
        services.AddSecurityWeb(configuration: webConfiguration.Security);
        cCoder.Mail.IServiceCollectionExtensions.AddMailWeb(
            services: services,
            configuration: webConfiguration.Mail);

        return services;
    }
}