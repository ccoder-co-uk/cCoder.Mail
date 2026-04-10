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
        string name = Unique("MailServer");
        MailServer expectedMailServer;
        MailServer actualMailServer;

        // When
        expectedMailServer = await CreateMailServerAsync(new
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

        actualMailServer = await GetMailServerAsync(expectedMailServer.Id);

        // Then
        actualMailServer.Should().NotBeNull();
        actualMailServer.Name.Should().Be(name);

        await DeleteMailServerAsync(expectedMailServer.Id);
        await Teardown(seededContext);
    }
}





