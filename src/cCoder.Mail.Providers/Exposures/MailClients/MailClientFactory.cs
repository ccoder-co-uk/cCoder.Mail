// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class MailClientFactory(
    Services.Orchestrations.IMailProviderOrchestrationService mailProviderOrchestrationService)
    : IMailClientFactory
{
    public IMailClient CreateMailClient(
        string providerName) =>
        mailProviderOrchestrationService.GetMailClient(
            providerName: providerName);

    public ValueTask<IMailClient> CreateMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default) =>
        mailProviderOrchestrationService.GetMailClientAsync(
            mailReceiverId: mailReceiverId,
            cancellationToken: cancellationToken);
}