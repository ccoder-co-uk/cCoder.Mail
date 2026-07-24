// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using Apps.Shared.Models;
using cCoder.Mail;
using cCoder.Mail.Models;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Eventing;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;
using MailConfig = cCoder.Mail.Models.Config;


namespace Mail.Web;

public class Program
{
    private static ILogger log = null!;

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        string coreConnection = builder.Configuration.GetConnectionString(name: "Core")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:Core is required.");

        string ssoConnection = builder.Configuration.GetConnectionString(name: "SSO")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:SSO is required.");

        Apps.Shared.Models.Config config = new();
        builder.Configuration.Bind(instance: config);
        builder.Services.AddSingleton(implementationInstance: config);

        builder.Services.AddSingleton(
implementationInstance: new MailConfig
{
    ConnectionStrings = new Dictionary<string, string>(dictionary: config.ConnectionStrings),
    Settings = new Dictionary<string, string>(dictionary: config.Settings),
    Services = new Dictionary<string, string>(dictionary: config.Services),
    DebugInfo = config.DebugInfo,
    LogSQL = config.LogSQL,
});

        builder.Services.AddEventing();

        builder.Services.AddSecurityApi(configAction: (services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services: services, connectionString: ssoConnection);

            securityConfig.UseAESHMMACPasswordEncryption(
services: services,
decryptionKey: builder.Configuration.GetSection(key: "Settings")["DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
services: builder.Services,
connectionString: coreConnection);

        builder.Services.AddMailWeb(newMailConfiguration: mailConfiguration =>
            ConfigureMailProviders(configuration: builder.Configuration, mailConfiguration: mailConfiguration));

        WebApplication app = builder.Build();
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseSession();
        app.UseStaticFiles();

        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
            {
                options.SwaggerEndpoint(url: "/swagger/Mail/swagger.json", name: "Mail API");
                options.SwaggerEndpoint(url: "/swagger/Core/swagger.json", name: "Core API");
                options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet(pattern: "/Health", handler: () => Results.Text(content: "OK"));
        app.MapGet(pattern: "/", handler: () => Results.Redirect(url: "/tools/index.html"));
        app.StartMailWeb(log: log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(errorHandler: HandleUnhandledException);
        app.Run();
    }

    private static async Task HandleUnhandledException(HttpContext context)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;

        context.Response.ContentType = "application/json";

        if (exception is null)
            return;

        log.LogError(message: "{Message}\n{StackTrace}", exception.Message, exception.StackTrace);

        await context.Response.WriteAsync(
text: "{ \"error\": \"" + exception.Message.Replace(oldValue: "\"", newValue: "\'") + "\" }");
    }

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
            .AddMicrosoftGraphSender(newMicrosoftGraphMailConfiguration: graphConfiguration => ConfigureGraph(configuration: configuration, graphConfiguration: graphConfiguration))
            .AddMicrosoftGraphReceiver(newMicrosoftGraphMailConfiguration: graphConfiguration => ConfigureGraph(configuration: configuration, graphConfiguration: graphConfiguration));
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
