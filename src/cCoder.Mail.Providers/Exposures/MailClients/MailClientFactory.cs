// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Exposures.MailClients;

internal sealed class MailClientFactory(
    Services.Foundations.IMailProviderService mailProviderService)
    : IMailClientFactory
{
    public IMailClient CreateMailClient(
        string providerName) =>
        mailProviderService.GetMailClient(
            providerName: providerName);

    public ValueTask<IMailClient> CreateMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default) =>
        mailProviderService.GetMailClientAsync(
            mailReceiverId: mailReceiverId,
            cancellationToken: cancellationToken);
}