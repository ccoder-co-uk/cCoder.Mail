// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface IMailProviderService
{
    Exposures.MailClients.IMailClient GetMailClient(
        string providerName);

    ValueTask<Exposures.MailClients.IMailClient> GetMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);

    ValueTask<string> GetMailReceiverProviderNameAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);
}