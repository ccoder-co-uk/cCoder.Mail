using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

public interface IMicrosoftGraphBroker
{
    Task<HttpClientBrokerResponse> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default);
}
