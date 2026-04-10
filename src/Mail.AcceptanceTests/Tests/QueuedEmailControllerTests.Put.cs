using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class QueuedEmailControllerTests
{
    [Fact]
    public async Task Put_UpdatesQueuedEmail()
    {
        // Given
        SeededQueuedEmailContext seededContext = await SeedDatabase();
        QueuedEmail createdQueuedEmail = await CreateQueuedEmailAsync(new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = Unique("QueuedEmail"),
            content = "Acceptance email content",
            to = "acceptance@example.com",
            cc = string.Empty,
            isBodyHtml = true,
            mailServerName = "Default",
        });
        string updatedSubject = Unique("UpdatedQueuedEmail");
        QueuedEmail actualQueuedEmail;

        // When
        await UpdateQueuedEmailAsync(createdQueuedEmail.Id, new
        {
            id = createdQueuedEmail.Id,
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = updatedSubject,
            content = "Updated email content",
            to = "acceptance@example.com",
            cc = "cc@example.com",
            isBodyHtml = true,
            mailServerName = "Default",
        });

        actualQueuedEmail = await GetQueuedEmailAsync(createdQueuedEmail.Id);

        // Then
        actualQueuedEmail.Subject.Should().Be(updatedSubject);

        await DeleteQueuedEmailAsync(createdQueuedEmail.Id);
        await Teardown(seededContext);
    }
}





