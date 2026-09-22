// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class Pop3MailboxService(
    IPop3MailReceiverBroker pop3MailReceiverBroker)
    : IPop3MailboxService
{
    public Task<string[][]> RetrieveMailboxReceiveRequestMessagesAsync(
        MailboxReceiveRequest mailboxReceiveRequest,
        CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: async () =>
            {
                ValidateMailboxReceiveRequestMessagesOnRetrieve(
                    inputs: [mailboxReceiveRequest, cancellationToken]);

                return await pop3MailReceiverBroker.ReceiveAsync(
                    request: mailboxReceiveRequest,
                    cancellationToken: cancellationToken);
            });
}