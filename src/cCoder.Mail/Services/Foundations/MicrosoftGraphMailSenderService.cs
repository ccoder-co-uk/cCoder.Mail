using System.Net.Http.Headers;
using System.Text.Json;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;

namespace cCoder.Mail.Services.Foundations;

internal sealed class MicrosoftGraphMailSenderService(
    MailConfiguration mailConfiguration,
    IMicrosoftGraphBroker microsoftGraphBroker)
    : IMicrosoftGraphMailSenderService
{
    private const string TenantIdVariableName = "CCODER_MAIL_GRAPH_TENANT_ID";
    private const string ClientIdVariableName = "CCODER_MAIL_GRAPH_CLIENT_ID";
    private const string ClientSecretVariableName = "CCODER_MAIL_GRAPH_CLIENT_SECRET";
    private const string GraphBaseUrlVariableName = "CCODER_MAIL_GRAPH_BASE_URL";
    private const string LoginBaseUrlVariableName = "CCODER_MAIL_GRAPH_LOGIN_BASE_URL";
    private const string DefaultGraphBaseUrl = "https://graph.microsoft.com/v1.0";
    private const string DefaultLoginBaseUrl = "https://login.microsoftonline.com";

    public async Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default)
    {
        MailSender sender = GetMailSender(email);
        string accessToken = await GetAccessTokenAsync(cancellationToken);
        using HttpRequestMessage message = new(HttpMethod.Post, BuildSendUrl(sender));
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        message.Content = JsonContent.Create(CreateSendPayload(email));

        HttpClientBrokerResponse response = await microsoftGraphBroker.SendAsync(message, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Microsoft Graph mail send failed: {response.Content}");
    }

    private static MailSender GetMailSender(QueuedEmail email)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        MailSender sender = email.MailSender;

        if (sender == null)
            throw new InvalidOperationException("No mail sender configuration could be found to send the email.");

        if (string.IsNullOrWhiteSpace(sender.User))
            throw new InvalidOperationException("Microsoft Graph sender user is required.");

        if (string.IsNullOrWhiteSpace(email.To))
            throw new InvalidOperationException("Email recipient is required.");

        return sender;
    }

    private string BuildSendUrl(MailSender sender)
    {
        string graphBaseUrl = ReadConfiguredValue(
            mailConfiguration.MicrosoftGraph.GraphBaseUrl,
            GraphBaseUrlVariableName)
            ?? DefaultGraphBaseUrl;

        return $"{graphBaseUrl.TrimEnd('/')}/users/{Uri.EscapeDataString(sender.User)}/sendMail";
    }

    private static object CreateSendPayload(QueuedEmail email) =>
        new
        {
            message = new
            {
                subject = email.Subject,
                body = new
                {
                    contentType = email.IsBodyHtml ? "HTML" : "Text",
                    content = email.Content ?? string.Empty,
                },
                toRecipients = Recipients(email.To),
                ccRecipients = Recipients(email.CC),
            },
            saveToSentItems = true,
        };

    private static object[] Recipients(string addresses) =>
        string.IsNullOrWhiteSpace(addresses)
            ? []
            : addresses
                .Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(address => new
                {
                    emailAddress = new
                    {
                        address,
                    },
                })
                .ToArray();

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, BuildTokenUrl())
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>(
                    "client_id",
                    ReadRequiredConfiguredValue(mailConfiguration.MicrosoftGraph.ClientId, ClientIdVariableName)),
                new KeyValuePair<string, string>(
                    "client_secret",
                    ReadRequiredConfiguredValue(mailConfiguration.MicrosoftGraph.ClientSecret, ClientSecretVariableName)),
                new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            ]),
        };

        HttpClientBrokerResponse response = await microsoftGraphBroker.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Microsoft Graph token request failed: {response.Content}");

        using JsonDocument document = JsonDocument.Parse(response.Content);

        return document.RootElement.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("Microsoft Graph token response did not include an access token.");
    }

    private string BuildTokenUrl()
    {
        string tenantId = ReadRequiredConfiguredValue(
            mailConfiguration.MicrosoftGraph.TenantId,
            TenantIdVariableName);
        string loginBaseUrl = ReadConfiguredValue(
            mailConfiguration.MicrosoftGraph.LoginBaseUrl,
            LoginBaseUrlVariableName)
            ?? DefaultLoginBaseUrl;

        return $"{loginBaseUrl.TrimEnd('/')}/{Uri.EscapeDataString(tenantId)}/oauth2/v2.0/token";
    }

    private static string ReadRequiredConfiguredValue(string configuredValue, string variableName) =>
        ReadConfiguredValue(configuredValue, variableName)
        ?? throw new InvalidOperationException($"{variableName} is required for Microsoft Graph mail.");

    private static string ReadConfiguredValue(string configuredValue, string variableName) =>
        string.IsNullOrWhiteSpace(configuredValue)
            ? ReadEnvironment(variableName)
            : configuredValue;

    private static string ReadEnvironment(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variableName)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
