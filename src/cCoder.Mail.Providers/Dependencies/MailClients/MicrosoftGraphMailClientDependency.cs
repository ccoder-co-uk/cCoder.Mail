// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data.Models.Mail;

namespace cCoder.Mail.Providers.Dependencies.MailClients;

internal sealed class MicrosoftGraphMailClientDependency : HttpClient
{
    private const string DefaultGraphBaseUrl =
        "https://graph.microsoft.com/v1.0";

    private const string DefaultLoginBaseUrl =
        "https://login.microsoftonline.com";

    internal async Task<(bool IsSuccessStatusCode, string Content)> SendEmailAsync(
        QueuedEmail email,
        string tenantId,
        string clientId,
        string clientSecret,
        string graphBaseUrl,
        string loginBaseUrl,
        CancellationToken cancellationToken = default)
    {
        string accessToken = await GetAccessTokenAsync(
            tenantId: tenantId,
            clientId: clientId,
            clientSecret: clientSecret,
            loginBaseUrl: loginBaseUrl,
            cancellationToken: cancellationToken);

        using HttpRequestMessage request = new(
            method: HttpMethod.Post,
            requestUri: BuildSendUrl(
                email: email,
                graphBaseUrl: graphBaseUrl));

        request.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: "Bearer",
            parameter: accessToken);

        request.Content = JsonContent.Create(
            inputValue: CreateSendPayload(email: email));

        return await SendRequestAsync(
            request: request,
            cancellationToken: cancellationToken);
    }

    internal async Task<(bool IsSuccessStatusCode, string Content)> ReceiveEmailAsync(
        string user,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int maximumMessages,
        string tenantId,
        string clientId,
        string clientSecret,
        string graphBaseUrl,
        string loginBaseUrl,
        CancellationToken cancellationToken = default)
    {
        string accessToken = await GetAccessTokenAsync(
            tenantId: tenantId,
            clientId: clientId,
            clientSecret: clientSecret,
            loginBaseUrl: loginBaseUrl,
            cancellationToken: cancellationToken);

        using HttpRequestMessage message = new(
            method: HttpMethod.Get,
            requestUri: BuildMessagesUrl(
                user: user,
                from: from,
                to: to,
                maximumMessages: maximumMessages,
                graphBaseUrl: graphBaseUrl));

        message.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: "Bearer",
            parameter: accessToken);

        return await SendRequestAsync(
            request: message,
            cancellationToken: cancellationToken);
    }

    private async Task<string> GetAccessTokenAsync(
        string tenantId,
        string clientId,
        string clientSecret,
        string loginBaseUrl,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(
            method: HttpMethod.Post,
            requestUri: BuildTokenUrl(
                tenantId: tenantId,
                loginBaseUrl: loginBaseUrl))
        {
            Content = new FormUrlEncodedContent(
                nameValueCollection:
                [
                    new KeyValuePair<string, string>(
                        key: "client_id",
                        value: ReadRequiredConfiguredValue(
                            configuredValue: clientId,
                            configurationName:
                                "Microsoft Graph client id")),
                    new KeyValuePair<string, string>(
                        key: "client_secret",
                        value: ReadRequiredConfiguredValue(
                            configuredValue: clientSecret,
                            configurationName:
                                "Microsoft Graph client secret")),
                    new KeyValuePair<string, string>(
                        key: "scope",
                        value:
                            "https://graph.microsoft.com/.default"),
                    new KeyValuePair<string, string>(
                        key: "grant_type",
                        value: "client_credentials"),
                ]),
        };

        (bool IsSuccessStatusCode, string Content) response = await SendRequestAsync(
            request: request,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                message:
                    $"Microsoft Graph token request failed: {response.Content}");
        }

        using JsonDocument document = JsonDocument.Parse(
            json: response.Content);

        return document.RootElement
            .GetProperty(propertyName: "access_token")
            .GetString()
            ?? throw new InvalidOperationException(
                message:
                    "Microsoft Graph token response did not include an access token.");
    }

    private async Task<(bool IsSuccessStatusCode, string Content)> SendRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await SendAsync(
            request: request,
            cancellationToken: cancellationToken);

        string content = await response.Content
            .ReadAsStringAsync(
                cancellationToken: cancellationToken);

        return (
            IsSuccessStatusCode: response.IsSuccessStatusCode,
            Content: content);
    }

    private static string BuildSendUrl(
        QueuedEmail email,
        string graphBaseUrl)
    {
        MailSender sender = email.MailSender
            ?? throw new InvalidOperationException(
                message:
                    "No mail sender configuration could be found to send the email.");

        graphBaseUrl =
            ReadConfiguredValue(
                configuredValue: graphBaseUrl)
            ?? DefaultGraphBaseUrl;

        return
            $"{graphBaseUrl.TrimEnd(trimChar: '/')}/users/{Uri.EscapeDataString(stringToEscape: sender.User)}/sendMail";
    }

    private static string BuildTokenUrl(
        string tenantId,
        string loginBaseUrl)
    {
        tenantId = ReadRequiredConfiguredValue(
            configuredValue: tenantId,
            configurationName: "Microsoft Graph tenant id");

        loginBaseUrl =
            ReadConfiguredValue(
                configuredValue: loginBaseUrl)
            ?? DefaultLoginBaseUrl;

        return
            $"{loginBaseUrl.TrimEnd(trimChar: '/')}/{Uri.EscapeDataString(stringToEscape: tenantId)}/oauth2/v2.0/token";
    }

    private static string BuildMessagesUrl(
        string user,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int maximumMessages,
        string graphBaseUrl)
    {
        graphBaseUrl =
            ReadConfiguredValue(
                configuredValue: graphBaseUrl)
            ?? DefaultGraphBaseUrl;

        List<string> query =
        [
            "$select=internetMessageId,subject,body,receivedDateTime,from,toRecipients,ccRecipients",
            $"$top={Math.Clamp(value: maximumMessages <= 0 ? 100 : maximumMessages, min: 1, max: 100)}",
            "$orderby=receivedDateTime desc",
        ];

        string filter = BuildFilter(from: from, to: to);

        if (!string.IsNullOrWhiteSpace(value: filter))
        {
            query.Add(
                item:
                    $"$filter={Uri.EscapeDataString(stringToEscape: filter)}");
        }

        return
            $"{graphBaseUrl.TrimEnd(trimChar: '/')}/users/{Uri.EscapeDataString(stringToEscape: user)}/mailFolders/inbox/messages"
            + $"?{string.Join(separator: "&", values: query)}";
    }

    private static string BuildFilter(
        DateTimeOffset? from,
        DateTimeOffset? to)
    {
        List<string> filters = [];

        if (from is not null)
        {
            filters.Add(
                item:
                    $"receivedDateTime ge {from.Value.UtcDateTime:O}");
        }

        if (to is not null)
        {
            filters.Add(
                item:
                    $"receivedDateTime le {to.Value.UtcDateTime:O}");
        }

        return string.Join(
            separator: " and ",
            values: filters);
    }

    private static object CreateSendPayload(
        QueuedEmail email) =>
        new
        {
            message = new
            {
                subject = email.Subject,
                body = new
                {
                    contentType =
                        email.IsBodyHtml
                            ? "HTML"
                            : "Text",
                    content = email.Content
                        ?? string.Empty,
                },
                toRecipients = Recipients(
                    addresses: email.To),
                ccRecipients = Recipients(
                    addresses: email.CC),
            },
            saveToSentItems = true,
        };

    private static object[] Recipients(
        string addresses) =>
        string.IsNullOrWhiteSpace(value: addresses)
            ? []
            : addresses
                .Split(
                    separator: [';', ','],
                    options:
                        StringSplitOptions.RemoveEmptyEntries
                        | StringSplitOptions.TrimEntries)
                .Select(
                    selector: address => new
                    {
                        emailAddress = new
                        {
                            address,
                        },
                    })
                .ToArray();

    private static string ReadRequiredConfiguredValue(
        string configuredValue,
        string configurationName) =>
        ReadConfiguredValue(
            configuredValue: configuredValue)
        ?? throw new InvalidOperationException(
            message:
                $"{configurationName} is required for Microsoft Graph mail.");

    private static string ReadConfiguredValue(
        string configuredValue) =>
        string.IsNullOrWhiteSpace(value: configuredValue)
            ? null
            : configuredValue;

    protected override void Dispose(bool disposing) =>
        base.Dispose(disposing: disposing);
}