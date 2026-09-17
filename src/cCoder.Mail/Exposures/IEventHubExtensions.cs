// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Mail.Services.Aggregations;

namespace cCoder.Mail;

public static class IEventHubExtensions
{
    public static IEventHub ListenToMailEvents(this IEventHub eventHub)
    {
        eventHub.ListenToEvent(
            name: "app_add",
            handler: (IAppAggregationService service, App app) =>
                service.AddAppAsync(newApp: app));

        eventHub.ListenToEvent(
            name: "app_update",
            handler: (IAppAggregationService service, App app) =>
                service.UpdateAppAsync(updatedApp: app));

        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IAppAggregationService service, App app) =>
                service.DeleteAsync(appId: app.Id));

        return eventHub;
    }
}