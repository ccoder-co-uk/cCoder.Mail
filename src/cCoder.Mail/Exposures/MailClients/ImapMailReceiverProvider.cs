// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using cCoder.Mail.Services.Foundations;

namespace cCoder.Mail.Exposures.MailClients;

internal sealed class ImapMailReceiverProvider(IImapMailReceiverService imapMailReceiverService)
    : IMailReceiverProvider
{
    public string ProviderName =>
        MailProviderNames.Imap;

    public Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default) =>
        imapMailReceiverService.ReceiveAsync(request: request, cancellationToken: cancellationToken);

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        imapMailReceiverService.ReceiveTopAsync(count: count, cancellationToken: cancellationToken);
}