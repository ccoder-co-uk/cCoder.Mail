using cCoder.Mail;
using cCoder.Mail.Models;
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
        builder.Services.AddMailHostedServices(mailConfiguration =>
            ConfigureMailProviders(builder.Configuration, mailConfiguration));

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
            "- MailReceiverHostedService -> IMailReceiverOrchestrationService.RunContinuouslyAsync every 1 minute",
            string.Empty,
            "Hosted event listeners:",
            "- app_add -> mail app setup",
            "- app_update -> mail app update",
            "- app_delete -> mail app cleanup");

    private static void ConfigureMailProviders(
        IConfiguration configuration,
        MailConfiguration mailConfiguration)
    {
        ConfigureRuntime(configuration, mailConfiguration);
        ConfigureMailbox(configuration, mailConfiguration.Pop3, "POP", 995);
        ConfigureMailbox(configuration, mailConfiguration.Imap, "IMAP", 993);

        mailConfiguration
            .AddSmtpSender()
            .AddPop3Receiver()
            .AddImapReceiver()
            .AddMicrosoftGraphSender(graphConfiguration => ConfigureGraph(configuration, graphConfiguration))
            .AddMicrosoftGraphReceiver(graphConfiguration => ConfigureGraph(configuration, graphConfiguration));
    }

    private static void ConfigureGraph(
        IConfiguration configuration,
        MicrosoftGraphMailConfiguration graphConfiguration)
    {
        graphConfiguration.TenantId = configuration["CCODER_MAIL_GRAPH_TENANT_ID"] ?? graphConfiguration.TenantId;
        graphConfiguration.ClientId = configuration["CCODER_MAIL_GRAPH_CLIENT_ID"] ?? graphConfiguration.ClientId;
        graphConfiguration.ClientSecret = configuration["CCODER_MAIL_GRAPH_CLIENT_SECRET"] ?? graphConfiguration.ClientSecret;
        graphConfiguration.GraphBaseUrl = configuration["CCODER_MAIL_GRAPH_BASE_URL"] ?? graphConfiguration.GraphBaseUrl;
        graphConfiguration.LoginBaseUrl = configuration["CCODER_MAIL_GRAPH_LOGIN_BASE_URL"]
            ?? configuration["CCODER_MAIL_GRAPH_LOGIN_URL"]
            ?? graphConfiguration.LoginBaseUrl;
        graphConfiguration.ReceiveUser = configuration["CCODER_MAIL_RECEIVE_USER"]
            ?? configuration["CCODER_MAIL_INTEGRATION_RECEIVE_USER"]
            ?? configuration["CCODER_MAIL_INTEGRATION_SEND_USER"]
            ?? configuration["CCODER_MAIL_INTEGRATION_SMTP_USER"]
            ?? graphConfiguration.ReceiveUser;
    }

    private static void ConfigureRuntime(
        IConfiguration configuration,
        MailConfiguration mailConfiguration)
    {
        mailConfiguration.IsMigrating =
            int.TryParse(configuration["MIGRATING"], out int result) && result == 1;
    }

    private static void ConfigureMailbox(
        IConfiguration configuration,
        MailboxReceiveConfiguration mailboxConfiguration,
        string providerPrefix,
        int defaultPort)
    {
        mailboxConfiguration.Host = configuration[$"CCODER_MAIL_{providerPrefix}_HOST"]
            ?? configuration["CCODER_MAIL_RECEIVE_HOST"]
            ?? mailboxConfiguration.Host;
        mailboxConfiguration.Port = int.TryParse(
            configuration[$"CCODER_MAIL_{providerPrefix}_PORT"]
            ?? configuration["CCODER_MAIL_RECEIVE_PORT"],
            out int port)
                ? port
                : defaultPort;
        mailboxConfiguration.EnableSSL = !bool.TryParse(
            configuration[$"CCODER_MAIL_{providerPrefix}_SSL"]
            ?? configuration["CCODER_MAIL_RECEIVE_SSL"],
            out bool enableSsl)
                || enableSsl;
        mailboxConfiguration.User = configuration[$"CCODER_MAIL_{providerPrefix}_USER"]
            ?? configuration["CCODER_MAIL_RECEIVE_USER"]
            ?? mailboxConfiguration.User;
        mailboxConfiguration.Password = configuration[$"CCODER_MAIL_{providerPrefix}_PASSWORD"]
            ?? configuration["CCODER_MAIL_RECEIVE_PASSWORD"]
            ?? mailboxConfiguration.Password;
    }
}
