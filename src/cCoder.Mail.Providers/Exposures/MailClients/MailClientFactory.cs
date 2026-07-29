// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class MailClientFactory(
    IEnumerable<IMailClient> mailClients,
    Services.Foundations.IMailProviderService mailProviderService)
    : IMailClientFactory
{
    private readonly IReadOnlyDictionary<string, IMailClient> mailClients =
        mailClients
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

    public IMailClient CreateMailClient(
        string providerName) =>
        mailClients.GetValueOrDefault(
            key: providerName
                ?? string.Empty)
        ?? throw new InvalidOperationException(
            message:
                $"No mail provider named '{providerName}' is registered.");

    public async ValueTask<IMailClient> CreateMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default) =>
        CreateMailClient(
            providerName:
                await mailProviderService
                    .GetMailReceiverProviderNameAsync(
                        mailReceiverId: mailReceiverId,
                        cancellationToken: cancellationToken));
}