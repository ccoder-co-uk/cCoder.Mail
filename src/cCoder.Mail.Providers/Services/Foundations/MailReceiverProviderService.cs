// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.Storages;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MailReceiverProviderService(
    IMailReceiverStorageBroker mailReceiverStorageBroker)
    : IMailReceiverProviderService
{
    public ValueTask<MailReceiver> RetrieveMailReceiverAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: async () =>
            {
                ValidateMailReceiverOnRetrieve(
                    inputs:
                    [
                        mailReceiverId,
                        cancellationToken
                    ]);

                return await SelectMailReceiverByIdAsync(
                    mailReceiverId: mailReceiverId,
                    cancellationToken: cancellationToken);
            });

    public ValueTask<string> RetrieveMailReceiverProviderNameAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: async () =>
            {
                ValidateMailReceiverProviderNameOnRetrieve(
                    inputs:
                    [
                        mailReceiverId,
                        cancellationToken
                    ]);

                MailReceiver mailReceiver =
                    await SelectMailReceiverByIdAsync(
                        mailReceiverId: mailReceiverId,
                        cancellationToken: cancellationToken);

                return mailReceiver.ProviderName;
            });

    private async ValueTask<MailReceiver> SelectMailReceiverByIdAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken) =>
        await mailReceiverStorageBroker
            .SelectMailReceiverByIdAsync(
                mailReceiverId: mailReceiverId,
                cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException(
            message:
                $"Mail receiver '{mailReceiverId}' was not found.");
}