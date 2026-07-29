// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Providers.Brokers.Storages;

internal interface IMailReceiverStorageBroker
{
    ValueTask<MailReceiver> SelectMailReceiverByIdAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default);
}