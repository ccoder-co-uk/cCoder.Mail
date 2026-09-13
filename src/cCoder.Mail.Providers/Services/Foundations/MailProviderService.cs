// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Exposures.MailClients;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MailProviderService(
    IMailReceiverStorageBroker mailReceiverStorageBroker,
    IMailClientRegistryBroker mailClientRegistryBroker)
    : IMailProviderService
{
    public IMailClient GetMailClient(
        string providerName) =>
        TryCatch(
            operation: () =>
            {
                ValidateMailClientOnGet(
                    inputs: [providerName]);

                return mailClientRegistryBroker.SelectMailClient(
                    providerName: providerName);
            });

    public ValueTask<IMailClient> GetMailClientAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: async () =>
            {
                ValidateMailClientOnGet(
                    inputs:
                        [
                            mailReceiverId,
                            cancellationToken
                        ]);

                string providerName =
                    await SelectMailReceiverProviderNameAsync(
                        mailReceiverId: mailReceiverId,
                        cancellationToken: cancellationToken);

                return mailClientRegistryBroker.SelectMailClient(
                    providerName: providerName);
            },
            isValueTask: true);

    public ValueTask<string> GetMailReceiverProviderNameAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: async () =>
            {
                ValidateMailReceiverProviderNameOnGet(
                    inputs:
                        [
                            mailReceiverId,
                            cancellationToken
                        ]);

                return await SelectMailReceiverProviderNameAsync(
                    mailReceiverId: mailReceiverId,
                    cancellationToken: cancellationToken);
            },
            isValueTask: true);

    private async ValueTask<string> SelectMailReceiverProviderNameAsync(
        Guid mailReceiverId,
        CancellationToken cancellationToken)
    {
        MailReceiver mailReceiver =
            await mailReceiverStorageBroker
                .SelectMailReceiverByIdAsync(
                    mailReceiverId: mailReceiverId,
                    cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException(
                message:
                    $"Mail receiver '{mailReceiverId}' was not found.");

        return mailReceiver.ProviderName;
    }
}