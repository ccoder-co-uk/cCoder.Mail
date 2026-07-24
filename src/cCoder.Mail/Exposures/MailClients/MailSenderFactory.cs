// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Exposures.MailClients;

internal sealed class MailSenderFactory(
    IEnumerable<IMailSenderProvider> senders,
    MailConfiguration mailConfiguration)
    : IMailSenderFactory
{
    private readonly IReadOnlyDictionary<string, IMailSenderProvider> senders =
        senders.ToDictionary(keySelector: sender => sender.ProviderName, comparer: StringComparer.OrdinalIgnoreCase);

    public IMailSenderProvider GetSender(string providerName) =>
        senders.GetValueOrDefault(key: mailConfiguration.ResolveSenderProviderName(providerName: providerName))
        ?? throw new InvalidOperationException(message: $"No mail sender provider named '{providerName}' is registered.");
}