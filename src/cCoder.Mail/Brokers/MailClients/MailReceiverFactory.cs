using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailReceiverFactory(
    IEnumerable<IMailReceiverProvider> receivers,
    MailConfiguration mailConfiguration)
    : IMailReceiverFactory
{
    private readonly IReadOnlyDictionary<string, IMailReceiverProvider> receivers =
        receivers.ToDictionary(receiver => receiver.ProviderName, StringComparer.OrdinalIgnoreCase);

    public IMailReceiverProvider GetReceiver(string providerName) =>
        receivers.GetValueOrDefault(mailConfiguration.ResolveReceiverProviderName(providerName))
        ?? throw new InvalidOperationException($"No mail receiver provider named '{providerName}' is registered.");
}
