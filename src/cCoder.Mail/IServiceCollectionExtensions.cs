using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Api.OData;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Exposures;
using cCoder.Mail.Exposures.EventHandlers;
using cCoder.Mail.Exposures.HostedServices;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Foundations.Events;
using cCoder.Mail.Services.Orchestrations;
using cCoder.Mail.Services.Processings;
using EventLibrary;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.Mail.Brokers.AuthorizationBroker;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;
using IJsonBroker = cCoder.Mail.Brokers.IJsonBroker;
using JsonBroker = cCoder.Mail.Brokers.JsonBroker;


namespace cCoder.Mail;

public static class IServiceCollectionExtensions
{
    public static void AddMail(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddEventHandlers();
    }

    public static void AddMailApi(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddMail();
        services.AddApi("Mail", ConfigureMailApiModel, builder, useFullSchemaIds: true);
    }

    public static void AddMailHostedServices(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddTransient<IMailSenderOrchestrationService, MailSenderOrchestrationService>();
        services.AddHostedService<MailSenderHostedService>();
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<MailServer>();
        services.AddEventingForType<QueuedEmail>();
        services.AddEventingForType<SentEmail>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IMailServerEventBroker, MailServerEventBroker>();
        services.AddTransient<IQueuedEmailEventBroker, QueuedEmailEventBroker>();
        services.AddTransient<ISentEmailEventBroker, SentEmailEventBroker>();
        services.AddTransient<IMailServerBroker, MailServerBroker>();
        services.AddTransient<IQueuedEmailBroker, QueuedEmailBroker>();
        services.AddTransient<ISentEmailBroker, SentEmailBroker>();
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IMailAppExposure, MailAppExposure>();
        services.AddTransient<IMailManagerExposure, MailManagerExposure>();
        services.AddTransient<IMailMetadataTypeService, MailMetadataTypeService>();
        services.AddTransient<Services.Foundations.Events.IEventHandlerService, Services.Foundations.Events.EventHandlerService>();
        services.AddTransient<IMailServerService, MailServerService>();
        services.AddTransient<IQueuedEmailService, QueuedEmailService>();
        services.AddTransient<ISentEmailService, SentEmailService>();
        services.AddTransient<IMailServerEventService, MailServerEventService>();
        services.AddTransient<IQueuedEmailEventService, QueuedEmailEventService>();
        services.AddTransient<ISentEmailEventService, SentEmailEventService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IAppOrchestrationService, AppOrchestrationService>();
        services.AddTransient<IMailSenderOrchestrationService, MailSenderOrchestrationService>();
        services.AddTransient<IMailServerOrchestrationService, MailServerOrchestrationService>();
        services.AddTransient<IQueuedEmailOrchestrationService, QueuedEmailOrchestrationService>();
        services.AddTransient<ISentEmailOrchestrationService, SentEmailOrchestrationService>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IMailEventHandlers, MailEventHandlers>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IMailServerEventProcessingService, MailServerEventProcessingService>();
        services.AddTransient<IMailServerProcessingService, MailServerProcessingService>();
        services.AddTransient<IQueuedEmailEventProcessingService, QueuedEmailEventProcessingService>();
        services.AddTransient<IQueuedEmailProcessingService, QueuedEmailProcessingService>();
        services.AddTransient<ISentEmailEventProcessingService, SentEmailEventProcessingService>();
        services.AddTransient<ISentEmailProcessingService, SentEmailProcessingService>();
    }

    private static void ConfigureMailApiModel(ODataConventionModelBuilder builder) =>
        new MailModelBuilder(builder).Configure();

    private static void AddApi(
        this IServiceCollection services,
        string routePrefix,
        Action<ODataConventionModelBuilder> configureModel,
        ODataConventionModelBuilder builder = null,
        bool useFullSchemaIds = false)
    {
        services.AddSingleton<Action<ODataConventionModelBuilder>>(configureModel);

        if (builder is not null)
            configureModel(builder);

        AddAspNet(services);

        if (builder is null)
            AddApiDocumentation(services, routePrefix, useFullSchemaIds);

        IEdmModel routeModel = BuildRouteModel(configureModel);
        DefaultODataBatchHandler batchHandler = new();

        services.AddControllers().AddOData(options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;
            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(1000)
                .AddRouteComponents($"Api/{routePrefix}", routeModel, batchHandler);

            if (builder is null)
                _ = options.AddRouteComponents("Api/Core", routeModel, batchHandler);
        });
    }

    private static void AddApiDocumentation(
        IServiceCollection services,
        string routePrefix,
        bool useFullSchemaIds)
    {
        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            AddSwaggerDocuments(options, routePrefix);
            options.DocInclusionPredicate(
                (documentName, apiDescription) =>
                    ShouldIncludeInDocument(documentName, apiDescription.RelativePath, routePrefix));

            if (useFullSchemaIds)
                options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Description = @"Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
            });
        });
    }

    private static void AddSwaggerDocuments(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        string routePrefix)
    {
        options.SwaggerDoc(routePrefix, new OpenApiInfo
        {
            Title = $"{routePrefix} API definition",
            Version = routePrefix,
        });
        options.SwaggerDoc("Core", new OpenApiInfo
        {
            Title = "Core API definition",
            Version = "Core",
        });
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Core API definition",
            Version = "v1",
        });
    }

    private static bool ShouldIncludeInDocument(
        string documentName,
        string relativePath,
        string routePrefix)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        if (string.Equals(documentName, "v1", StringComparison.OrdinalIgnoreCase))
            documentName = "Core";

        string path = NormalizePath(relativePath);

        return string.Equals(documentName, "Core", StringComparison.OrdinalIgnoreCase)
            ? MatchesContextRoute(path, "Core")
            : MatchesContextRoute(path, routePrefix);
    }

    private static bool MatchesContextRoute(string path, string context)
    {
        string prefix = $"/Api/{context}";
        return path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
            || path.StartsWith($"{prefix}/", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string relativePath) =>
        relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildRouteModel(Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(builder);
        return builder.GetEdmModel();
    }

    private static void AddAspNet(IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped(
            typeof(HttpContext),
            ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext ?? new DefaultHttpContext());
        services.AddScoped(typeof(HttpRequest), ctx => ctx.GetRequiredService<HttpContext>().Request);
        services.AddSession();
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(60);
        });
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddRazorPages();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });
        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}







