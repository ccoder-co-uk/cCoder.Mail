// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;

namespace cCoder.Mail.Brokers.Events;

internal class EventHubBroker(IEventHub eventHub) : IEventHubBroker
{
    public void ListenToEvent<T, TService>(string eventName, Func<TService, T, ValueTask> handler) =>
        eventHub.ListenToEvent(name: eventName, handler: handler);
}