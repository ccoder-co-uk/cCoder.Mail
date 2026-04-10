using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class QueuedEmailControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetQueuedEmailCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfQueuedEmails()
    {
        // Given

        // When
        IReadOnlyList<QueuedEmail> actualQueuedEmails = await GetQueuedEmailsAsync(1);

        // Then
        actualQueuedEmails.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsQueuedEmailById()
    {
        // Given
        SeededQueuedEmailContext seededContext = await SeedDatabase();
        string subject = Unique("QueuedEmail");
        QueuedEmail expectedQueuedEmail = await CreateQueuedEmailAsync(new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject,
            content = "Acceptance email content",
            to = "acceptance@example.com",
            cc = string.Empty,
            isBodyHtml = true,
            mailServerName = "Default",
        });
        QueuedEmail actualQueuedEmail;

        // When
        actualQueuedEmail = await GetQueuedEmailAsync(expectedQueuedEmail.Id);

        // Then
        actualQueuedEmail.Id.Should().Be(expectedQueuedEmail.Id);
        actualQueuedEmail.Subject.Should().Be(subject);

        await DeleteQueuedEmailAsync(expectedQueuedEmail.Id);
        await Teardown(seededContext);
    }
}





