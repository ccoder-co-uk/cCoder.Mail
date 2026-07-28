// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.OData;
using cCoder.Mail.Models;
using cCoder.Data;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;

namespace cCoder.Mail;

internal static class MailServiceCollectionConfigurationExtensions
{
    public static void ConfigureMailApiModel(this ODataConventionModelBuilder builder) =>
        new MailModelBroker(builder: builder).Configure();

    internal static void RegisterConfiguration(
        this IServiceCollection services,
        MailConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);
        services.AddSingleton(implementationInstance: configuration);

        if (!string.IsNullOrWhiteSpace(
            value: configuration.ConnectionString))
        {
            services.AddData(
                configuration: new cCoder.Data.Models.DataConfiguration
                {
                    ConnectionString = configuration.ConnectionString,
                    DebugInfo = configuration.DebugInfo,
                    LogSQL = configuration.LogSQL,
                });
        }

        services.AddEventProviders(eventProviders: configuration.EventProviders);
    }

    internal static void AddConfiguredApi(
        this IServiceCollection services,
        MailConfiguration newMailConfiguration,
        string documentName,
        Action<ODataConventionModelBuilder> configureModel,
        ODataConventionModelBuilder builder = null,
        bool useFullSchemaIds = false)
    {
        services.AddSingleton<Action<ODataConventionModelBuilder>>(implementationInstance: configureModel);

        if (builder is not null)
        {
            configureModel(obj: builder);
        }

        AddAspNet(services: services);

        if (builder is null)
        {
            AddApiDocumentation(services: services, documentName: documentName, newMailConfiguration: newMailConfiguration, useFullSchemaIds: useFullSchemaIds);
        }

        IEdmModel routeModel = BuildRouteModel(configureModel: configureModel);
        DefaultODataBatchHandler batchHandler = new();

        string rootPath = string.IsNullOrWhiteSpace(value: newMailConfiguration.RootPath)
            ? $"Api/{documentName}"
            : newMailConfiguration.RootPath;

        services.AddControllers()
            .AddOData(setupAction: options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;

            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(maxTopValue: 1000)
                .AddRouteComponents(routePrefix: rootPath, model: routeModel, batchHandler: batchHandler);

        });
    }

    private static void AddApiDocumentation(
        IServiceCollection services,
        string documentName,
        MailConfiguration newMailConfiguration,
        bool useFullSchemaIds) =>
        services.AddSwaggerGen(setupAction: options =>
        {
            options.ResolveConflictingActions(resolver: apiDescriptions => apiDescriptions.First());
            AddSwaggerDocuments(options: options, documentName: documentName, newMailConfiguration: newMailConfiguration);

            options.DocInclusionPredicate(
predicate: (swaggerDocumentName, apiDescription) =>
                    ShouldIncludeInDocument(
swaggerDocumentName: swaggerDocumentName,
relativePath: apiDescription.RelativePath,
documentName: documentName,
configuration: newMailConfiguration));

            if (useFullSchemaIds)
            {
                options.CustomSchemaIds(schemaIdSelector: type => type.FullName?.Replace(oldChar: '+', newChar: '.') ?? type.Name);
            }

            options.AddSecurityDefinition(name: "bearer", securityScheme: new OpenApiSecurityScheme
            {
                Description = @"Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
            });
        });

    private static void AddSwaggerDocuments(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        string documentName,
        MailConfiguration newMailConfiguration) =>
        options.SwaggerDoc(name: documentName, info: new OpenApiInfo
        {
            Title = $"{documentName} API definition",
            Version = documentName,
        });

    private static bool ShouldIncludeInDocument(
        string swaggerDocumentName,
        string relativePath,
        string documentName,
        MailConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(value: relativePath))
        {
            return false;
        }

        string path = NormalizePath(relativePath: relativePath);

        string rootPath = string.IsNullOrWhiteSpace(value: configuration.RootPath)
            ? $"Api/{documentName}"
            : configuration.RootPath;

        return string.Equals(
            a: swaggerDocumentName,
            b: documentName,
            comparisonType: StringComparison.OrdinalIgnoreCase)
            && MatchesContextRoute(
                path: path,
                rootPath: rootPath);
    }

    private static bool MatchesContextRoute(string path, string rootPath)
    {
        string normalizedPath = NormalizePath(relativePath: rootPath);

        return path.Equals(value: normalizedPath, comparisonType: StringComparison.OrdinalIgnoreCase)
            || path.StartsWith(value: $"{normalizedPath}/", comparisonType: StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string relativePath) =>
        relativePath.StartsWith(value: '/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildRouteModel(Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(obj: builder);
        return builder.GetEdmModel();
    }

    private static void AddAspNet(IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddScoped(
serviceType: typeof(HttpContext),
implementationFactory: ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext ?? new DefaultHttpContext());

        services.AddScoped(serviceType: typeof(HttpRequest), implementationFactory: ctx => ctx.GetRequiredService<HttpContext>()
            .Request);

        services.AddSession();

        services.AddHsts(configureOptions: options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(minutes: 60);
        });

        services.AddMvc(setupAction: options => options.EnableEndpointRouting = false);
        services.AddRazorPages();

        services.Configure<KestrelServerOptions>(configureOptions: options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });

        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}