// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
namespace cCoder.Mail.Providers.Exposures.MailClients;

public interface IMailClientFactory
{
    IMailClient CreateMailClient(string providerName);

    ValueTask<IMailClient> CreateMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);
}