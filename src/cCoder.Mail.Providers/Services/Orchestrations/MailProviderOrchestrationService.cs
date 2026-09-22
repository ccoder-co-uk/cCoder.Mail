// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Exposures.MailClients;
using cCoder.Mail.Providers.Services.Foundations;

namespace cCoder.Mail.Providers.Services.Orchestrations;

internal sealed partial class MailProviderOrchestrationService(
    IMailProviderService mailProviderService,
    IMailReceiverProviderService mailReceiverProviderService)
    : IMailProviderOrchestrationService
{
    public IMailClient GetMailClient(
        string providerName) =>
        TryCatch(
            operation: () =>
            {
                ValidateMailClientOnGet(
                    inputs: [providerName]);

                return mailProviderService.GetMailClient(
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
                    await mailReceiverProviderService
                        .RetrieveMailReceiverProviderNameAsync(
                            mailReceiverId: mailReceiverId,
                            cancellationToken: cancellationToken);

                return mailProviderService.GetMailClient(
                    providerName: providerName);
            });
}