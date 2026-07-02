using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailReceiverFactory(
    IEnumerable<IMailReceiverProvider> receivers,
    MailConfiguration mailConfiguration)
    : IMailReceiverFactory
{
    public IMailReceiverProvider GetReceiver(string providerName)
    {
        string name = string.IsNullOrWhiteSpace(providerName)
            ? mailConfiguration.DefaultReceiverProviderName
            : providerName;
        string resolvedName = mailConfiguration.ReceiverProviders.TryGetValue(name, out string configuredName)
            ? configuredName
            : name;

        return receivers.FirstOrDefault(receiver =>
            string.Equals(receiver.ProviderName, resolvedName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"No mail receiver provider named '{name}' is registered.");
    }
}
