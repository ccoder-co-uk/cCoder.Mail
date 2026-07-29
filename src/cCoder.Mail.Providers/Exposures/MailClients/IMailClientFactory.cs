// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Exposures.MailClients;

public interface IMailClientFactory
{
    IMailClient CreateMailClient(string providerName);

    ValueTask<IMailClient> CreateMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);
}