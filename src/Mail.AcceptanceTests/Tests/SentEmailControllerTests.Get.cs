using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class SentEmailControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetSentEmailCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfSentEmails()
    {
        // Given

        // When
        IReadOnlyList<SentEmail> actualSentEmails = await GetSentEmailsAsync(1);

        // Then
        actualSentEmails.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsSentEmailById()
    {
        // Given
        SeededSentEmailContext seededContext = await SeedDatabase();
        string subject = Unique("SentEmail");
        SentEmail expectedSentEmail = await CreateSentEmailAsync(new
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
        SentEmail actualSentEmail;

        // When
        actualSentEmail = await GetSentEmailAsync(expectedSentEmail.Id);

        // Then
        actualSentEmail.Id.Should().Be(expectedSentEmail.Id);
        actualSentEmail.Subject.Should().Be(subject);

        await DeleteSentEmailAsync(expectedSentEmail.Id);
        await Teardown(seededContext);
    }
}





