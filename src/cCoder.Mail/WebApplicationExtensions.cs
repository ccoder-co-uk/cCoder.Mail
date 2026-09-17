// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.Mail.Services.Foundations;


namespace cCoder.Mail;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "Mail";

    public static WebApplication StartMailWeb(this WebApplication app, ILogger log = null) =>
        app.UseMailExposure(log: log);

    public static WebApplication StartMailHostedServices(this WebApplication app) =>
        app.UseMailExposure();

    private static WebApplication UseMailExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation(message: "Initialising Mail");
        PopulateMetadataTypeCache(app: app);
        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(scope: MetadataScope))
        {
            metadataTypeCache.Set(
scope: MetadataScope,
typeSetPayloads: app.Services
                    .GetRequiredService<IMailMetadataTypeService>()
                .GetKnownMetadata()
                .Select(selector: static metadata => JsonSerializer.Serialize(value: metadata)));
        }
    }
}