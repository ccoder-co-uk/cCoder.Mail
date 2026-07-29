// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Mail;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Mail.Providers.Brokers.Storages;

internal sealed class MailReceiverStorageBroker(
    ICoreContextFactory coreContextFactory)
    : IMailReceiverStorageBroker
{
    public async ValueTask<MailReceiver> SelectMailReceiverByIdAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default)
    {
        await using CoreDataContext context =
            coreContextFactory.CreateCoreContext();

        return await context.MailReceivers
            .SingleOrDefaultAsync(
                predicate: receiver =>
                    receiver.Id == mailReceiverId,
                cancellationToken: cancellationToken);
    }
}