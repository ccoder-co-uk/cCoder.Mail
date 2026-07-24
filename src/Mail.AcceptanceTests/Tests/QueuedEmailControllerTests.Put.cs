// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        QueuedEmail createdQueuedEmail = await CreateQueuedEmailAsync(payload: new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = Unique(prefix: "QueuedEmail"),
            content = "Acceptance email content",
            to = "acceptance@example.com",
            cc = string.Empty,
            isBodyHtml = true,
            mailServerName = "Default",
        });

        string updatedSubject = Unique(prefix: "UpdatedQueuedEmail");
        QueuedEmail actualQueuedEmail;

        // When

        await UpdateQueuedEmailAsync(id: createdQueuedEmail.Id, payload: new
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

        actualQueuedEmail = await GetQueuedEmailAsync(id: createdQueuedEmail.Id);

        // Then

        actualQueuedEmail.Subject.Should()
            .Be(expected: updatedSubject);

        await DeleteQueuedEmailAsync(id: createdQueuedEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}