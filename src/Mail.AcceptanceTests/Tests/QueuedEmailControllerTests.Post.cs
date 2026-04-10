using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class QueuedEmailControllerTests
{
    [Fact]
    public async Task Post_CreatesQueuedEmail()
    {
        // Given
        SeededQueuedEmailContext seededContext = await SeedDatabase();
        string subject = Unique("QueuedEmail");
        QueuedEmail expectedQueuedEmail;
        QueuedEmail actualQueuedEmail;

        // When
        expectedQueuedEmail = await CreateQueuedEmailAsync(new
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

        actualQueuedEmail = await GetQueuedEmailAsync(expectedQueuedEmail.Id);

        // Then
        actualQueuedEmail.Subject.Should().Be(subject);

        await DeleteQueuedEmailAsync(expectedQueuedEmail.Id);
        await Teardown(seededContext);
    }
}





