using cCoder.Data.Models.CMS;
using cCoder.Mail.Brokers.Events;
using cCoder.Mail.Services.Orchestrations;

namespace cCoder.Mail.Services.Foundations.Events;

internal class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents()
    {
        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
        ListenToAppDeleteEvents();
    }

    private void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_add",
            (service, app) => service.AddAsync(app));

    private void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_update",
            (service, app) => service.UpdateAsync(app));

    private void ListenToAppDeleteEvents() =>
        eventHubBroker.ListenToEvent<App, IAppOrchestrationService>(
            "app_delete",
            (service, app) => service.DeleteAsync(app.Id));
}
