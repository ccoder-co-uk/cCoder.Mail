// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Eventing;
using cCoder.Security;
using cCoder.Security.Data.EF;
using Mail.Web.Models;

namespace Mail.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfiguration> configure = null)
    {
        AppConfiguration webConfiguration = new();
        configuration.Bind(instance: webConfiguration);
        configure?.Invoke(obj: webConfiguration);

        services.AddEventingWeb(configuration: webConfiguration.Eventing);
        services.AddData(configuration: webConfiguration.CoreData);
        services.AddSecurityData(configuration: webConfiguration.SecurityData);
        services.AddSecurityWeb(configuration: webConfiguration.Security);
        cCoder.Mail.IServiceCollectionExtensions.AddMailWeb(
            services: services,
            configuration: webConfiguration.Mail);

        return services;
    }
}