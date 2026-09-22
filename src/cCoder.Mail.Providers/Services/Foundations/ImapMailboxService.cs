// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class ImapMailboxService(
    IImapMailReceiverBroker imapMailReceiverBroker)
    : IImapMailboxService
{
    public Task<string[]> RetrieveMailboxReceiveRequestMessagesAsync(
        MailboxReceiveRequest mailboxReceiveRequest,
        CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: async () =>
            {
                ValidateMailboxReceiveRequestMessagesOnRetrieve(
                    inputs: [mailboxReceiveRequest, cancellationToken]);

                return await imapMailReceiverBroker.ReceiveAsync(
                    request: mailboxReceiveRequest,
                    cancellationToken: cancellationToken);
            });
}