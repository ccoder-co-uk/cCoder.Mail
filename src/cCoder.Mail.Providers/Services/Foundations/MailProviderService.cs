// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Exposures.MailClients;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MailProviderService(
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

}