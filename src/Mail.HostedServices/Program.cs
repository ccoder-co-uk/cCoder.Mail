using cCoder.Mail;
using cCoder.Eventing;
using MailConfig = cCoder.Mail.Models.Config;

namespace Mail.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string coreConnection = builder.Configuration.GetConnectionString("Core")
            ?? throw new InvalidOperationException("ConnectionStrings:Core is required.");

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            builder.Services,
            coreConnection);

        MailConfig config = new();
        builder.Configuration.Bind(config);
        builder.Services.AddSingleton(config);
        builder.Services.AddEventing();
        builder.Services.AddMailHostedServices();

        WebApplication app = builder.Build();

        app.MapGet("/", (IHostEnvironment environment) =>
            Results.Text(BuildHostedServicesReport(environment), "text/plain"));
        app.MapGet("/Health", () => Results.Text("Healthy"));

        app.StartMailHostedServices();
        app.Run();
    }

    private static string BuildHostedServicesReport(IHostEnvironment environment) =>
        string.Join(
            Environment.NewLine,
            "cCoder.Mail Hosted Services",
            "Status: Healthy",
            $"Environment: {environment.EnvironmentName}",
            "Health: /Health",
            string.Empty,
            "Hosted background services:",
            "- MailSenderHostedService -> IMailSenderOrchestrationService.RunContinuouslyAsync every 1 minute",
            string.Empty,
            "Hosted event listeners:",
            "- app_add -> mail app setup",
            "- app_update -> mail app update",
            "- app_delete -> mail app cleanup");
}
