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
    public async Task Post_CreatesQueuedEmail()
    {
        // Given
        SeededQueuedEmailContext seededContext = await SeedDatabase();
        string subject = Unique(prefix: "QueuedEmail");
        QueuedEmail expectedQueuedEmail;
        QueuedEmail actualQueuedEmail;

        // When

        expectedQueuedEmail = await CreateQueuedEmailAsync(payload: new
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

        actualQueuedEmail = await GetQueuedEmailAsync(id: expectedQueuedEmail.Id);

        // Then

        actualQueuedEmail.Subject.Should()
            .Be(expected: subject);

        await DeleteQueuedEmailAsync(id: expectedQueuedEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}