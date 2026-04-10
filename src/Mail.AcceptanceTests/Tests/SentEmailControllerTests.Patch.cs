using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class SentEmailControllerTests
{
    [Fact]
    public async Task Patch_UpdatesSentEmail()
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
        string updatedSubject = Unique("PatchedSentEmail");
        SentEmail actualSentEmail;

        // When
        await PatchSentEmailAsync(createdSentEmail.Id, new
        {
            subject = updatedSubject,
            cc = "patched@example.com",
        });

        actualSentEmail = await GetSentEmailAsync(createdSentEmail.Id);

        // Then
        actualSentEmail.Subject.Should().Be(updatedSubject);
        actualSentEmail.CC.Should().Be("patched@example.com");

        await DeleteSentEmailAsync(createdSentEmail.Id);
        await Teardown(seededContext);
    }
}





