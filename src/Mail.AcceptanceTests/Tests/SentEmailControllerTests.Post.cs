using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class SentEmailControllerTests
{
    [Fact]
    public async Task Post_CreatesSentEmail()
    {
        // Given
        SeededSentEmailContext seededContext = await SeedDatabase();
        string subject = Unique("SentEmail");
        SentEmail expectedSentEmail;
        SentEmail actualSentEmail;

        // When
        expectedSentEmail = await CreateSentEmailAsync(new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject,
            content = "Acceptance email content",
            to = "acceptance@example.com",
            cc = string.Empty,
            isBodyHtml = true,
            from = "acceptance@example.com",
            sentOn = DateTimeOffset.UtcNow,
        });

        actualSentEmail = await GetSentEmailAsync(expectedSentEmail.Id);

        // Then
        actualSentEmail.Subject.Should().Be(subject);

        await DeleteSentEmailAsync(expectedSentEmail.Id);
        await Teardown(seededContext);
    }
}





