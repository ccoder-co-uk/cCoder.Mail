// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Mail.HostedServices.Hosting;

namespace Mail.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddMailHostedServicesApplication(configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseMailHostedServicesApplication();
        app.Run();
    }
}