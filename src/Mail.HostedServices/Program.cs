// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Mail;

namespace Mail.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddHostedServices(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.Services
            .GetRequiredService<IEventHub>()
            .ListenToMailEvents();

        app.UseMailHostedServicesApplication()
            .Run();
    }
}