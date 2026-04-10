namespace cCoder.Mail.Brokers.Events;

public interface IEventHubBroker
{
    void ListenToEvent<T, TService>(string eventName, Func<TService, T, ValueTask> handler);
}
