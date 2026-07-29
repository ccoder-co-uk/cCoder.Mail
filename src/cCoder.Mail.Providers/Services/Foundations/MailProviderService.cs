// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.Storages;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MailProviderService(
    IMailReceiverStorageBroker mailReceiverStorageBroker)
    : IMailProviderService
{
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

                MailReceiver mailReceiver =
                    await mailReceiverStorageBroker
                        .SelectMailReceiverByIdAsync(
                            mailReceiverId: mailReceiverId,
                            cancellationToken: cancellationToken)
                    ?? throw new InvalidOperationException(
                        message:
                            $"Mail receiver '{mailReceiverId}' was not found.");

                return mailReceiver.ProviderName;
            },
            isValueTask: true);
}