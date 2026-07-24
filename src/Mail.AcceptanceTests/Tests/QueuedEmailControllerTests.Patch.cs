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
    public async Task Patch_UpdatesQueuedEmail()
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

        string updatedSubject = Unique(prefix: "PatchedQueuedEmail");
        QueuedEmail actualQueuedEmail;

        // When

        await PatchQueuedEmailAsync(id: createdQueuedEmail.Id, payload: new
        {
            subject = updatedSubject,
            cc = "patched@example.com",
        });

        actualQueuedEmail = await GetQueuedEmailAsync(id: createdQueuedEmail.Id);

        // Then

        actualQueuedEmail.Subject.Should()
            .Be(expected: updatedSubject);

        actualQueuedEmail.CC.Should()
            .Be(expected: "patched@example.com");

        await DeleteQueuedEmailAsync(id: createdQueuedEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}