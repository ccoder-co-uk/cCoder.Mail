// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface IMailProviderService
{
    ValueTask<string> GetMailReceiverProviderNameAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);
}