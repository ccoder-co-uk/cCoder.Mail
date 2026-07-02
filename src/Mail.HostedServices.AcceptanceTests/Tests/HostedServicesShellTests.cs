using System.Net;
using FluentAssertions;
using HostedServices.AcceptanceTests.Infrastructure;
using Xunit;

namespace HostedServices.AcceptanceTests.Tests;

[Collection(HostedServicesAcceptanceCollection.Name)]
public sealed partial class HostedServicesShellTests(HostedServicesAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private async Task<string> GetOkContentAsync(string path)
    {
        using HttpResponseMessage response = await Client.GetAsync(path);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        return content;
    }
}
