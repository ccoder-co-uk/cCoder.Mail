using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class ReceivedEmailControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private string BaseUrl { get; } = "/Api/Core/ReceivedEmail/Receive";

    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private async Task<ReceivedEmail[]> ReceiveEmailsAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        return JsonSerializer.Deserialize<ReceivedEmail[]>(content, JsonOptions)
            ?? throw new InvalidOperationException("Expected received email payload.");
    }

    private async Task<ReceivedEmail[]> ReceiveTopEmailsAsync(int count)
    {
        using HttpResponseMessage response = await Client.GetAsync($"/Api/Core/ReceivedEmail/ReceiveTop/{count}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        return JsonSerializer.Deserialize<ReceivedEmail[]>(content, JsonOptions)
            ?? throw new InvalidOperationException("Expected received email payload.");
    }
}
