// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Exposures.MailClients;

namespace cCoder.Mail.Providers.Services.Orchestrations;

internal interface IMailProviderOrchestrationService
{
    IMailClient GetMailClient(string providerName);

    ValueTask<IMailClient> GetMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);
}