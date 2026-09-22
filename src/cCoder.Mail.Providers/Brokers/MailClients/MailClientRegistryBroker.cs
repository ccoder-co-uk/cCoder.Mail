// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Exposures.MailClients;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal sealed class MailClientRegistryBroker(
    IServiceProvider serviceProvider)
    : IMailClientRegistryBroker
{
    private readonly IReadOnlyDictionary<string, IMailClient> mailClients =
        serviceProvider
            .GetServices<IMailClient>()
            .SelectMany(
                collectionSelector: mailClient =>
                    mailClient.GetProviderNames(),
                resultSelector: (mailClient, providerName) =>
                    new KeyValuePair<string, IMailClient>(
                        key: providerName,
                        value: mailClient))
            .ToDictionary(
                keySelector: mailClient => mailClient.Key,
                elementSelector: mailClient => mailClient.Value,
                comparer: StringComparer.OrdinalIgnoreCase);

    public IMailClient SelectMailClient(
        string providerName) =>
        mailClients.GetValueOrDefault(
            key: providerName
                ?? string.Empty)
        ?? throw new InvalidOperationException(
            message:
                $"No mail provider named '{providerName}' is registered.");
}