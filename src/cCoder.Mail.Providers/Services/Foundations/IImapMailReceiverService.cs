// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

public interface IImapMailReceiverService
{
    Task<ReceivedEmail[]> ReceiveMailReceiverAsync(
        Guid mailReceiverId,
        int maximumMessages,
        CancellationToken cancellationToken = default);
}