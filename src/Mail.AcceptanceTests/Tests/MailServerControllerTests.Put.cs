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
    public async Task Put_UpdatesMailServer()
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

        string updatedName = Unique(prefix: "UpdatedMailServer");
        MailServer actualMailServer;

        // When

        await UpdateMailServerAsync(id: createdMailServer.Id, payload: new
        {
            id = createdMailServer.Id,
            appId = seededContext.AppId,
            name = updatedName,
            user = "acceptance",
            password = "password",
            host = "smtp.acceptance.local",
            fromEmail = "acceptance@example.com",
            port = 587,
            enableSSL = true,
        });

        actualMailServer = await GetMailServerAsync(id: createdMailServer.Id);

        // Then

        actualMailServer.Should()
            .NotBeNull();

        actualMailServer.Name.Should()
            .Be(expected: updatedName);

        actualMailServer.Port.Should()
            .Be(expected: 587);

        await DeleteMailServerAsync(id: createdMailServer.Id);
        await Teardown(seededContext: seededContext);
    }
}