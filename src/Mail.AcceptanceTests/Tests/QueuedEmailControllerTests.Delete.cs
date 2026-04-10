using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class QueuedEmailControllerTests
{
    [Fact]
    public async Task Delete_RemovesQueuedEmail()
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
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteQueuedEmailAsync(createdQueuedEmail.Id);
        actualReadStatusCode = await GetQueuedEmailStatusCodeAsync(createdQueuedEmail.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





