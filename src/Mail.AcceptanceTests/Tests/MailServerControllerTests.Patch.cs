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
    public async Task Patch_UpdatesMailServer()
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

        string updatedName = Unique(prefix: "PatchedMailServer");
        MailServer actualMailServer;

        // When

        await PatchMailServerAsync(id: createdMailServer.Id, payload: new
        {
            name = updatedName,
            port = 2525,
        });

        actualMailServer = await GetMailServerAsync(id: createdMailServer.Id);

        // Then

        actualMailServer.Should()
            .NotBeNull();

        actualMailServer.Name.Should()
            .Be(expected: updatedName);

        actualMailServer.Port.Should()
            .Be(expected: 2525);

        await DeleteMailServerAsync(id: createdMailServer.Id);
        await Teardown(seededContext: seededContext);
    }
}