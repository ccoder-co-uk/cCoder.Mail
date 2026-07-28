// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Mail.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddMailHostedServices(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseMailHostedServicesApplication()
            .Run();
    }
}