// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed class MicrosoftGraphBroker : IMicrosoftGraphBroker
{
    private static readonly HttpClient HttpClient = new();

    public async Task<HttpClientBrokerResponse> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await HttpClient.SendAsync(request: request, cancellationToken: cancellationToken);
        string content = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);

        return new HttpClientBrokerResponse(IsSuccessStatusCode: response.IsSuccessStatusCode, Content: content);
    }
}