using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class QueuedEmailControllerTests
{
    [Fact]
    public async Task Patch_UpdatesQueuedEmail()
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
        string updatedSubject = Unique("PatchedQueuedEmail");
        QueuedEmail actualQueuedEmail;

        // When
        await PatchQueuedEmailAsync(createdQueuedEmail.Id, new
        {
            subject = updatedSubject,
            cc = "patched@example.com",
        });

        actualQueuedEmail = await GetQueuedEmailAsync(createdQueuedEmail.Id);

        // Then
        actualQueuedEmail.Subject.Should().Be(updatedSubject);
        actualQueuedEmail.CC.Should().Be("patched@example.com");

        await DeleteQueuedEmailAsync(createdQueuedEmail.Id);
        await Teardown(seededContext);
    }
}





