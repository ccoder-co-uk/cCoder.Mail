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
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteMailServerAsync(createdMailServer.Id);
        actualReadStatusCode = await GetMailServerStatusCodeAsync(createdMailServer.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





