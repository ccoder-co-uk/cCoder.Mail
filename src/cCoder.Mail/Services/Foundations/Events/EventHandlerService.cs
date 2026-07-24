// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Services.Orchestrations;

namespace cCoder.Mail.Services.Foundations.Events;

internal partial class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents() =>
        TryCatch(operation: () =>
    {
        ValidateListenToAllEvents(inputs: []);

        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
        ListenToAppDeleteEvents();
    });

    private void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
eventName: "app_add",
handler: (service, app) => service.AddAsync(app: app));

    private void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
eventName: "app_update",
handler: (service, app) => service.UpdateAsync(app: app));

    private void ListenToAppDeleteEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
eventName: "app_delete",
handler: (service, app) => service.DeleteAsync(appId: app.Id));
}