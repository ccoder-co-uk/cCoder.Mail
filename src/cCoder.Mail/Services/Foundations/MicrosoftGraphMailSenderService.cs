// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http.Headers;
using System.Text.Json;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Exposures;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MicrosoftGraphMailSenderService(
    IMailConfigurationExposure mailConfigurationExposure,
    IMicrosoftGraphBroker microsoftGraphBroker)
    : IMicrosoftGraphMailSenderService
{
    private readonly MailConfiguration mailConfiguration =
        mailConfigurationExposure.GetMailConfiguration();

    private const string DefaultGraphBaseUrl = "https://graph.microsoft.com/v1.0";

    private const string DefaultLoginBaseUrl = "https://login.microsoftonline.com";

    public Task SendQueuedEmailAsync(QueuedEmail email, CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
    {
        ValidateSendQueuedEmailAsync(inputs: [email, cancellationToken]);

        MailSender sender = GetMailSender(email: email);
        string accessToken = await GetAccessTokenAsync(cancellationToken: cancellationToken);
        using HttpRequestMessage message = new(method: HttpMethod.Post, requestUri: BuildSendUrl(sender: sender));
        message.Headers.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: accessToken);
        message.Content = JsonContent.Create(inputValue: CreateSendPayload(newQueuedEmail: email));

        HttpClientBrokerResponse response = await microsoftGraphBroker.SendAsync(request: message, cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(message: $"Microsoft Graph mail send failed: {response.Content}");
        }
    }, isTask: true);

    private static MailSender GetMailSender(QueuedEmail email)
    {
        if (email == null)
        {
            throw new ArgumentNullException(paramName: nameof(email));
        }

        MailSender sender = email.MailSender;

        if (sender == null)
        {
            throw new InvalidOperationException(message: "No mail sender configuration could be found to send the email.");
        }

        if (string.IsNullOrWhiteSpace(value: sender.User))
        {
            throw new InvalidOperationException(message: "Microsoft Graph sender user is required.");
        }

        if (string.IsNullOrWhiteSpace(value: email.To))
        {
            throw new InvalidOperationException(message: "Email recipient is required.");
        }

        return sender;
    }

    private string BuildSendUrl(MailSender sender)
    {
        string graphBaseUrl = ReadConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.GraphBaseUrl)
            ?? DefaultGraphBaseUrl;

        return $"{graphBaseUrl.TrimEnd(trimChar: '/')}/users/{Uri.EscapeDataString(stringToEscape: sender.User)}/sendMail";
    }

    private static object CreateSendPayload(QueuedEmail newQueuedEmail) =>
        new
        {
            message = new
            {
                subject = newQueuedEmail.Subject,
                body = new
                {
                    contentType = newQueuedEmail.IsBodyHtml ? "HTML" : "Text",
                    content = newQueuedEmail.Content ?? string.Empty,
                },
                toRecipients = Recipients(addresses: newQueuedEmail.To),
                ccRecipients = Recipients(addresses: newQueuedEmail.CC),
            },
            saveToSentItems = true,
        };

    private static object[] Recipients(string addresses) =>
        string.IsNullOrWhiteSpace(value: addresses)
            ? []
            : addresses
                .Split(separator: [';', ','], options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(selector: address => new
        {
            emailAddress = new
            {
                address,
            },
        })
        .ToArray();

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Post, requestUri: BuildTokenUrl())
        {
            Content = new FormUrlEncodedContent(
nameValueCollection: [
                new KeyValuePair<string, string>(
key: "client_id",
value: ReadRequiredConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.ClientId, configurationName: "Microsoft Graph client id")),
                new KeyValuePair<string, string>(
key: "client_secret",
value: ReadRequiredConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.ClientSecret, configurationName: "Microsoft Graph client secret")),
                new KeyValuePair<string, string>(key: "scope", value: "https://graph.microsoft.com/.default"),
                new KeyValuePair<string, string>(key: "grant_type", value: "client_credentials"),
            ]),
        };

        HttpClientBrokerResponse response = await microsoftGraphBroker.SendAsync(request: request, cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(message: $"Microsoft Graph token request failed: {response.Content}");
        }

        using JsonDocument document = JsonDocument.Parse(json: response.Content);

        return document.RootElement.GetProperty(propertyName: "access_token")
            .GetString()
            ?? throw new InvalidOperationException(message: "Microsoft Graph token response did not include an access token.");
    }

    private string BuildTokenUrl()
    {
        string tenantId = ReadRequiredConfiguredValue(
configuredValue: mailConfiguration.MicrosoftGraph.TenantId,
configurationName: "Microsoft Graph tenant id");

        string loginBaseUrl = ReadConfiguredValue(configuredValue: mailConfiguration.MicrosoftGraph.LoginBaseUrl)
            ?? DefaultLoginBaseUrl;

        return $"{loginBaseUrl.TrimEnd(trimChar: '/')}/{Uri.EscapeDataString(stringToEscape: tenantId)}/oauth2/v2.0/token";
    }

    private static string ReadRequiredConfiguredValue(string configuredValue, string configurationName) =>
        ReadConfiguredValue(configuredValue: configuredValue)
        ?? throw new InvalidOperationException(message: $"{configurationName} is required for Microsoft Graph mail.");

    private static string ReadConfiguredValue(string configuredValue) =>
        string.IsNullOrWhiteSpace(value: configuredValue) ? null : configuredValue;
}