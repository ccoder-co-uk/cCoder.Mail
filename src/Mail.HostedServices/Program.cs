// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail;
using cCoder.Mail.Models;
using cCoder.Eventing;
using MailConfig = cCoder.Mail.Models.Config;

namespace Mail.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        string coreConnection = builder.Configuration.GetConnectionString(name: "Core")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:Core is required.");

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
services: builder.Services,
connectionString: coreConnection);

        MailConfig config = new();
        builder.Configuration.Bind(instance: config);
        builder.Services.AddSingleton(implementationInstance: config);
        builder.Services.AddEventing();

        builder.Services.AddMailHostedServices(configure: mailConfiguration =>
            ConfigureMailProviders(configuration: builder.Configuration, mailConfiguration: mailConfiguration));

        WebApplication app = builder.Build();

        app.MapGet(pattern: "/", handler: (IHostEnvironment environment) =>
            Results.Text(content: BuildHostedServicesReport(environment: environment), contentType: "text/plain"));

        app.MapGet(pattern: "/Health", handler: () => Results.Text(content: "Healthy"));

        app.StartMailHostedServices();
        app.Run();
    }

    private static string BuildHostedServicesReport(IHostEnvironment environment) =>
        string.Join(
separator: Environment.NewLine,
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
        ConfigureRuntime(configuration: configuration, mailConfiguration: mailConfiguration);
        ConfigureMailbox(configuration: configuration, mailboxConfiguration: mailConfiguration.Pop3, providerPrefix: "POP", defaultPort: 995);
        ConfigureMailbox(configuration: configuration, mailboxConfiguration: mailConfiguration.Imap, providerPrefix: "IMAP", defaultPort: 993);

        mailConfiguration
            .AddSmtpSender()
            .AddPop3Receiver()
            .AddImapReceiver()
            .AddMicrosoftGraphSender(configure: graphConfiguration => ConfigureGraph(configuration: configuration, graphConfiguration: graphConfiguration))
            .AddMicrosoftGraphReceiver(configure: graphConfiguration => ConfigureGraph(configuration: configuration, graphConfiguration: graphConfiguration));
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
            int.TryParse(s: configuration["MIGRATING"], result: out int result) && result == 1;
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
s: configuration[$"CCODER_MAIL_{providerPrefix}_PORT"]
            ?? configuration["CCODER_MAIL_RECEIVE_PORT"],
result: out int port)
                ? port
                : defaultPort;

        mailboxConfiguration.EnableSSL = !bool.TryParse(
value: configuration[$"CCODER_MAIL_{providerPrefix}_SSL"]
            ?? configuration["CCODER_MAIL_RECEIVE_SSL"],
result: out bool enableSsl)
                || enableSsl;

        mailboxConfiguration.User = configuration[$"CCODER_MAIL_{providerPrefix}_USER"]
            ?? configuration["CCODER_MAIL_RECEIVE_USER"]
            ?? mailboxConfiguration.User;

        mailboxConfiguration.Password = configuration[$"CCODER_MAIL_{providerPrefix}_PASSWORD"]
            ?? configuration["CCODER_MAIL_RECEIVE_PASSWORD"]
            ?? mailboxConfiguration.Password;
    }
}