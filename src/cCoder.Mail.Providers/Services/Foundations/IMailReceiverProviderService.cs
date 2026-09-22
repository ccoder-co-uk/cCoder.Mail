// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface IMailReceiverProviderService
{
    ValueTask<cCoder.Data.Models.Mail.MailReceiver> RetrieveMailReceiverAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);

    ValueTask<string> RetrieveMailReceiverProviderNameAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);
}