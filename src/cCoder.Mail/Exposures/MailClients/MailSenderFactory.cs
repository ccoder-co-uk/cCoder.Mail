using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures.MailClients;

internal sealed class MailSenderFactory(
    IEnumerable<IMailSenderProvider> senders,
    MailConfiguration mailConfiguration)
    : IMailSenderFactory
{
    private readonly IReadOnlyDictionary<string, IMailSenderProvider> senders =
        senders.ToDictionary(sender => sender.ProviderName, StringComparer.OrdinalIgnoreCase);

    public IMailSenderProvider GetSender(string providerName) =>
        senders.GetValueOrDefault(mailConfiguration.ResolveSenderProviderName(providerName))
        ?? throw new InvalidOperationException($"No mail sender provider named '{providerName}' is registered.");
}
