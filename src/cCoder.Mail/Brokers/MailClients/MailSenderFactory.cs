using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MailSenderFactory(
    IEnumerable<IMailSenderProvider> senders,
    MailConfiguration mailConfiguration)
    : IMailSenderFactory
{
    public IMailSenderProvider GetSender(string providerName)
    {
        string name = string.IsNullOrWhiteSpace(providerName)
            ? mailConfiguration.DefaultSenderProviderName
            : providerName;
        string resolvedName = ResolveProviderName(providerName, name);

        return senders.FirstOrDefault(sender =>
            string.Equals(sender.ProviderName, resolvedName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"No mail sender provider named '{name}' is registered.");
    }

    private string ResolveProviderName(string providerName, string name)
    {
        if (mailConfiguration.SenderProviders.TryGetValue(name, out string configuredName))
            return configuredName;

        if (string.IsNullOrWhiteSpace(providerName)
            || name.Contains('.', StringComparison.Ordinal)
            || name.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return mailConfiguration.DefaultSenderProviderName;
        }

        return name;
    }
}
