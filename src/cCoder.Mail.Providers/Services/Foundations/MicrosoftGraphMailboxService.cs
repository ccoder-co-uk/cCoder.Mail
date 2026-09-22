// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MicrosoftGraphMailboxService(
    IMicrosoftGraphBroker microsoftGraphBroker)
    : IMicrosoftGraphMailboxService
{
    public Task<MicrosoftGraphMailboxRequest> RetrieveMicrosoftGraphMailboxRequestAsync(
        MicrosoftGraphMailboxRequest microsoftGraphMailboxRequest,
        CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: async () =>
            {
                ValidateMicrosoftGraphMailboxRequestOnRetrieve(
                    inputs: [microsoftGraphMailboxRequest, cancellationToken]);

                (bool IsSuccessStatusCode, string Content) response =
                    await microsoftGraphBroker.ReceiveEmailAsync(
                        user: microsoftGraphMailboxRequest.MailboxReceiveRequest.User,
                        from: microsoftGraphMailboxRequest.MailboxReceiveRequest.From,
                        to: microsoftGraphMailboxRequest.MailboxReceiveRequest.To,
                        maximumMessages: microsoftGraphMailboxRequest.MailboxReceiveRequest.MaximumMessages,
                        tenantId: microsoftGraphMailboxRequest.MailProviderConfiguration.TenantId,
                        clientId: microsoftGraphMailboxRequest.MailProviderConfiguration.ClientId,
                        clientSecret: microsoftGraphMailboxRequest.MailProviderConfiguration.ClientSecret,
                        graphBaseUrl: microsoftGraphMailboxRequest.MailProviderConfiguration.GraphBaseUrl,
                        loginBaseUrl: microsoftGraphMailboxRequest.MailProviderConfiguration.LoginBaseUrl,
                        cancellationToken: cancellationToken);

                microsoftGraphMailboxRequest.IsSuccessStatusCode =
                    response.IsSuccessStatusCode;

                microsoftGraphMailboxRequest.Content = response.Content;

                return microsoftGraphMailboxRequest;
            });
}