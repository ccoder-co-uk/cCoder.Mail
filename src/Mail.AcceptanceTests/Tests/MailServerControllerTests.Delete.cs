// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class MailServerControllerTests
{
    [Fact]
    public async Task Delete_RemovesMailServer()
    {
        // Given
        SeededMailServerContext seededContext = await SeedDatabase();

        MailServer createdMailServer = await CreateMailServerAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique(prefix: "MailServer"),
            user = "acceptance",
            password = "password",
            host = "smtp.acceptance.local",
            fromEmail = "acceptance@example.com",
            port = 25,
            enableSSL = false,
        });

        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteMailServerAsync(id: createdMailServer.Id);
        actualReadStatusCode = await GetMailServerStatusCodeAsync(id: createdMailServer.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}