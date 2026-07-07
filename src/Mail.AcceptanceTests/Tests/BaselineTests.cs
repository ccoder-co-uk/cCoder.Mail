using System.Net;
using System.Text.Json;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class BaselineTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private async Task<JsonElement> GetBaselineAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync("/Api/Mail/Baseline");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonDocument.Parse(content).RootElement.Clone();
    }
}
