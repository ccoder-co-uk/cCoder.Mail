using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class SentEmailControllerTests
{
    [Fact]
    public async Task Delete_RemovesSentEmail()
    {
        // Given
        SeededSentEmailContext seededContext = await SeedDatabase();
        SentEmail createdSentEmail = await CreateSentEmailAsync(new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = Unique("SentEmail"),
            content = "Acceptance email content",
            to = "acceptance@example.com",
            cc = string.Empty,
            isBodyHtml = true,
            from = "acceptance@example.com",
            sentOn = DateTimeOffset.UtcNow,
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteSentEmailAsync(createdSentEmail.Id);
        actualReadStatusCode = await GetSentEmailStatusCodeAsync(createdSentEmail.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





