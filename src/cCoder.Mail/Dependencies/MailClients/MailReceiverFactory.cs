// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Dependencies.MailClients;

internal sealed class MailReceiverFactory(
    IEnumerable<IMailReceiverProvider> receivers,
    MailConfiguration mailConfiguration)
    : IMailReceiverFactory
{
    private readonly IReadOnlyDictionary<string, IMailReceiverProvider> receivers =
        receivers.ToDictionary(keySelector: receiver => receiver.ProviderName, comparer: StringComparer.OrdinalIgnoreCase);

    public IMailReceiverProvider GetReceiver(string providerName) =>
        receivers.GetValueOrDefault(key: mailConfiguration.ResolveReceiverProviderName(providerName: providerName))
        ?? throw new InvalidOperationException(message: $"No mail receiver provider named '{providerName}' is registered.");
}