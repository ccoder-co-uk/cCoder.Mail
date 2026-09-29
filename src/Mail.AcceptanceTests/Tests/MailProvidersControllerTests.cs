// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using cCoder.Mail.Models;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class MailProvidersControllerTests(
    WebAcceptanceFixture fixture)
{
    [Theory]
    [InlineData("/Api/Mail/MailProviders")]
    [InlineData("/Api/Mail/MailProviders/Senders")]
    [InlineData("/Api/Mail/MailProviders/Receivers")]
    public async Task MailProviders_WhenRequested_ReturnConfiguredProvidersAsync(
        string requestUri)
    {
        // Given

        // When
        using HttpResponseMessage response = await fixture.Client.GetAsync(
            requestUri: requestUri);

        MailProviderSummary[] providers = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<MailProviderSummary[]>() ?? []
            : [];

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK);

        providers.Should()
            .NotBeEmpty();
    }

    [Theory]
    [InlineData("/Api/Mail/MailProviders")]
    [InlineData("/Api/Mail/MailProviders/Senders")]
    [InlineData("/Api/Mail/MailProviders/Receivers")]
    public async Task MailProviders_WhenProviderFails_ReturnServerErrorAsync(
        string requestUri)
    {
        // Given
        using HttpClient client = fixture.CreateFailingProviderClient();

        // When
        using HttpResponseMessage response = await client.GetAsync(
            requestUri: requestUri);

        // Then
        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.InternalServerError);
    }
}