// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MicrosoftGraphMailSenderService(
    MailProviderConfiguration configuration,
    IMicrosoftGraphBroker microsoftGraphBroker)
    : IMicrosoftGraphMailSenderService
{
    public Task SendQueuedEmailAsync(
        QueuedEmail queuedEmail,
        CancellationToken cancellationToken = default) =>
        TryCatch(
            operation: async () =>
            {
                ValidateSendQueuedEmailAsync(
                    inputs: [queuedEmail, cancellationToken]);

                ValidateMailSender(email: queuedEmail);

                (bool IsSuccessStatusCode, string Content) response =
                    await microsoftGraphBroker.SendEmailAsync(
                        email: queuedEmail,
                        tenantId: configuration.TenantId,
                        clientId: configuration.ClientId,
                        clientSecret: configuration.ClientSecret,
                        graphBaseUrl: configuration.GraphBaseUrl,
                        loginBaseUrl: configuration.LoginBaseUrl,
                        cancellationToken: cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        message:
                            $"Microsoft Graph mail send failed: {response.Content}");
                }
            },
            isTask: true);

    private static void ValidateMailSender(
        QueuedEmail email)
    {
        ArgumentNullException.ThrowIfNull(
            argument: email);

        MailSender sender = email.MailSender
            ?? throw new InvalidOperationException(
                message:
                    "No mail sender configuration could be found to send the email.");

        if (string.IsNullOrWhiteSpace(value: sender.User))
        {
            throw new InvalidOperationException(
                message:
                    "Microsoft Graph sender user is required.");
        }

        if (string.IsNullOrWhiteSpace(value: email.To))
        {
            throw new InvalidOperationException(
                message: "Email recipient is required.");
        }
    }
}