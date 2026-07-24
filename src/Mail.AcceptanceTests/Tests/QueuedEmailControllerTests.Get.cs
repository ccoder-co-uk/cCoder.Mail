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
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetQueuedEmailCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfQueuedEmails()
    {
        // Given

        // When
        IReadOnlyList<QueuedEmail> actualQueuedEmails = await GetQueuedEmailsAsync(top: 1);

        // Then

        actualQueuedEmails.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsQueuedEmailById()
    {
        // Given
        SeededQueuedEmailContext seededContext = await SeedDatabase();
        string subject = Unique(prefix: "QueuedEmail");

        QueuedEmail expectedQueuedEmail = await CreateQueuedEmailAsync(payload: new
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
        actualQueuedEmail = await GetQueuedEmailAsync(id: expectedQueuedEmail.Id);

        // Then

        actualQueuedEmail.Id.Should()
            .Be(expected: expectedQueuedEmail.Id);

        actualQueuedEmail.Subject.Should()
            .Be(expected: subject);

        await DeleteQueuedEmailAsync(id: expectedQueuedEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}