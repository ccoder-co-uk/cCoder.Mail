using System.Security;
using Apps.Shared;
using Apps.Shared.Models;
using cCoder.Mail;
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
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string coreConnection = builder.Configuration.GetConnectionString("Core")
            ?? throw new InvalidOperationException("ConnectionStrings:Core is required.");

        string ssoConnection = builder.Configuration.GetConnectionString("SSO")
            ?? throw new InvalidOperationException("ConnectionStrings:SSO is required.");

        Config config = new();
        builder.Configuration.Bind(config);
        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton(
            new MailConfig
            {
                ConnectionStrings = new Dictionary<string, string>(config.ConnectionStrings),
                Settings = new Dictionary<string, string>(config.Settings),
                Services = new Dictionary<string, string>(config.Services),
                DebugInfo = config.DebugInfo,
                LogSQL = config.LogSQL,
            });
        builder.Services.AddEventing();

        builder.Services.AddSecurityApi((services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services, ssoConnection);
            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                builder.Configuration.GetSection("Settings")["DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            builder.Services,
            coreConnection);

        builder.Services.AddMailWeb();

        WebApplication app = builder.Build();
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseSession();
        app.UseStaticFiles();

        app.UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/Mail/swagger.json", "Mail API");
                options.SwaggerEndpoint("/swagger/Core/swagger.json", "Core API");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet("/Health", () => Results.Text("OK"));
        app.MapGet("/", () => Results.Redirect("/tools/index.html"));
        app.StartMailWeb(log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(HandleUnhandledException);
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

        log.LogError("{Message}\n{StackTrace}", exception.Message, exception.StackTrace);
        await context.Response.WriteAsync(
            "{ \"error\": \"" + exception.Message.Replace("\"", "\'") + "\" }");
    }
}


