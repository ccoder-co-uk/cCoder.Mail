// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Services.Foundations;

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class MailClientFactory(
    IEnumerable<IMailClient> mailClients,
    IMailReceiverProviderService mailReceiverProviderService)
    : IMailClientFactory,
      cCoder.CodeAnalysis.Exposures.ICompositionExposure
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
        SelectMailClient(providerName: providerName);

    public async ValueTask<IMailClient> CreateMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default) =>
        SelectMailClient(
            providerName: await mailReceiverProviderService
                .RetrieveMailReceiverProviderNameAsync(
                    mailReceiverId: mailReceiverId,
                    cancellationToken: cancellationToken));

    private IMailClient SelectMailClient(
        string providerName) =>
        mailClients.GetValueOrDefault(
            key: providerName
                ?? string.Empty)
        ?? throw new InvalidOperationException(
            message:
                $"No mail provider named '{providerName}' is registered.");
}