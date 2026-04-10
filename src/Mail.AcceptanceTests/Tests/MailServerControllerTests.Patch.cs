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
        MailServer createdMailServer = await CreateMailServerAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("MailServer"),
            user = "acceptance",
            password = "password",
            host = "smtp.acceptance.local",
            fromEmail = "acceptance@example.com",
            port = 25,
            enableSSL = false,
        });
        string updatedName = Unique("PatchedMailServer");
        MailServer actualMailServer;

        // When
        await PatchMailServerAsync(createdMailServer.Id, new
        {
            name = updatedName,
            port = 2525,
        });

        actualMailServer = await GetMailServerAsync(createdMailServer.Id);

        // Then
        actualMailServer.Should().NotBeNull();
        actualMailServer.Name.Should().Be(updatedName);
        actualMailServer.Port.Should().Be(2525);

        await DeleteMailServerAsync(createdMailServer.Id);
        await Teardown(seededContext);
    }
}





