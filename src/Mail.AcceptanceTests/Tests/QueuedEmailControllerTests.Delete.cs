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
    public async Task Delete_RemovesQueuedEmail()
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

        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteQueuedEmailAsync(id: createdQueuedEmail.Id);
        actualReadStatusCode = await GetQueuedEmailStatusCodeAsync(id: createdQueuedEmail.Id);

        // Then

        actualStatusCode.Should()
            .Be(expected: 200);

        actualReadStatusCode.Should()
            .Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}