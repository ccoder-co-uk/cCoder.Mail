using System;
using System.Text.Json;
using cCoder.Data.Exposures;
using cCoder.Mail.Exposures.EventHandlers;
using cCoder.Mail.Services.Foundations;


namespace cCoder.Mail;

public static class WebApplicationExtensions
{
    private const string MetadataScope = "Mail";

    public static WebApplication UseMailExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation("Initialising Mail");
        PopulateMetadataTypeCache(app);
        return app;
    }

    public static WebApplication UseMailEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IMailEventHandlers handlers in services.GetServices<IMailEventHandlers>())
            handlers.ListenToAllEvents();

        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(MetadataScope))
        {
            metadataTypeCache.Set(
                MetadataScope,
                app.Services
                    .GetRequiredService<IMailMetadataTypeService>()
                    .GetKnownMetadata()
                    .Select(static metadata => JsonSerializer.Serialize(metadata)));
        }
    }
}
