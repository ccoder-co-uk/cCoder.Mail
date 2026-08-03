// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Dependencies.MailClients;

internal sealed class MicrosoftGraphMailClientDependency : HttpClient
{
    private const string DefaultGraphBaseUrl =
        "https://graph.microsoft.com/v1.0";

    private const string DefaultLoginBaseUrl =
        "https://login.microsoftonline.com";

    internal async Task<HttpClientBrokerResponse> SendEmailAsync(
        QueuedEmail email,
        MailProviderConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        string accessToken = await GetAccessTokenAsync(
            configuration: configuration,
            cancellationToken: cancellationToken);

        using HttpRequestMessage request = new(
            method: HttpMethod.Post,
            requestUri: BuildSendUrl(
                email: email,
                configuration: configuration));

        request.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: "Bearer",
            parameter: accessToken);

        request.Content = JsonContent.Create(
            inputValue: CreateSendPayload(email: email));

        return await SendRequestAsync(
            request: request,
            cancellationToken: cancellationToken);
    }

    internal async Task<HttpClientBrokerResponse> ReceiveEmailAsync(
        MailboxReceiveRequest request,
        MailProviderConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        string accessToken = await GetAccessTokenAsync(
            configuration: configuration,
            cancellationToken: cancellationToken);

        using HttpRequestMessage message = new(
            method: HttpMethod.Get,
            requestUri: BuildMessagesUrl(
                request: request,
                configuration: configuration));

        message.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: "Bearer",
            parameter: accessToken);

        return await SendRequestAsync(
            request: message,
            cancellationToken: cancellationToken);
    }

    private async Task<string> GetAccessTokenAsync(
        MailProviderConfiguration configuration,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(
            method: HttpMethod.Post,
            requestUri: BuildTokenUrl(
                configuration: configuration))
        {
            Content = new FormUrlEncodedContent(
                nameValueCollection:
                [
                    new KeyValuePair<string, string>(
                        key: "client_id",
                        value: ReadRequiredConfiguredValue(
                            configuredValue: configuration.ClientId,
                            configurationName:
                                "Microsoft Graph client id")),
                    new KeyValuePair<string, string>(
                        key: "client_secret",
                        value: ReadRequiredConfiguredValue(
                            configuredValue: configuration.ClientSecret,
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

        HttpClientBrokerResponse response = await SendRequestAsync(
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

    private async Task<HttpClientBrokerResponse> SendRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await SendAsync(
            request: request,
            cancellationToken: cancellationToken);

        string content = await response.Content
            .ReadAsStringAsync(
                cancellationToken: cancellationToken);

        return new HttpClientBrokerResponse(
            IsSuccessStatusCode: response.IsSuccessStatusCode,
            Content: content);
    }

    private static string BuildSendUrl(
        QueuedEmail email,
        MailProviderConfiguration configuration)
    {
        MailSender sender = email.MailSender
            ?? throw new InvalidOperationException(
                message:
                    "No mail sender configuration could be found to send the email.");

        string graphBaseUrl =
            ReadConfiguredValue(
                configuredValue: configuration.GraphBaseUrl)
            ?? DefaultGraphBaseUrl;

        return
            $"{graphBaseUrl.TrimEnd(trimChar: '/')}/users/{Uri.EscapeDataString(stringToEscape: sender.User)}/sendMail";
    }

    private static string BuildTokenUrl(
        MailProviderConfiguration configuration)
    {
        string tenantId = ReadRequiredConfiguredValue(
            configuredValue: configuration.TenantId,
            configurationName: "Microsoft Graph tenant id");

        string loginBaseUrl =
            ReadConfiguredValue(
                configuredValue: configuration.LoginBaseUrl)
            ?? DefaultLoginBaseUrl;

        return
            $"{loginBaseUrl.TrimEnd(trimChar: '/')}/{Uri.EscapeDataString(stringToEscape: tenantId)}/oauth2/v2.0/token";
    }

    private static string BuildMessagesUrl(
        MailboxReceiveRequest request,
        MailProviderConfiguration configuration)
    {
        string graphBaseUrl =
            ReadConfiguredValue(
                configuredValue: configuration.GraphBaseUrl)
            ?? DefaultGraphBaseUrl;

        List<string> query =
        [
            "$select=internetMessageId,subject,body,receivedDateTime,from,toRecipients,ccRecipients",
            $"$top={Math.Clamp(value: request.MaximumMessages <= 0 ? 100 : request.MaximumMessages, min: 1, max: 100)}",
            "$orderby=receivedDateTime desc",
        ];

        string filter = BuildFilter(request: request);

        if (!string.IsNullOrWhiteSpace(value: filter))
        {
            query.Add(
                item:
                    $"$filter={Uri.EscapeDataString(stringToEscape: filter)}");
        }

        return
            $"{graphBaseUrl.TrimEnd(trimChar: '/')}/users/{Uri.EscapeDataString(stringToEscape: request.User)}/mailFolders/inbox/messages"
            + $"?{string.Join(separator: "&", values: query)}";
    }

    private static string BuildFilter(
        MailboxReceiveRequest request)
    {
        List<string> filters = [];

        if (request.From is not null)
        {
            filters.Add(
                item:
                    $"receivedDateTime ge {request.From.Value.UtcDateTime:O}");
        }

        if (request.To is not null)
        {
            filters.Add(
                item:
                    $"receivedDateTime le {request.To.Value.UtcDateTime:O}");
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
}