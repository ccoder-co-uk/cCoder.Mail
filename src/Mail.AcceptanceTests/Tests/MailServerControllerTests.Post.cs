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
    public async Task Post_CreatesMailServer()
    {
        // Given
        SeededMailServerContext seededContext = await SeedDatabase();
        string name = Unique(prefix: "MailServer");
        MailServer expectedMailServer;
        MailServer actualMailServer;

        // When

        expectedMailServer = await CreateMailServerAsync(payload: new
        {
            appId = seededContext.AppId,
            name,
            user = "acceptance",
            password = "password",
            host = "smtp.acceptance.local",
            fromEmail = "acceptance@example.com",
            port = 25,
            enableSSL = false,
        });

        actualMailServer = await GetMailServerAsync(id: expectedMailServer.Id);

        // Then

        actualMailServer.Should()
            .NotBeNull();

        actualMailServer.Name.Should()
            .Be(expected: name);

        await DeleteMailServerAsync(id: expectedMailServer.Id);
        await Teardown(seededContext: seededContext);
    }
}