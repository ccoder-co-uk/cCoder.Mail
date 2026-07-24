// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class SentEmailControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetSentEmailCountAsync();

        // Then

        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfSentEmails()
    {
        // Given

        // When
        IReadOnlyList<SentEmail> actualSentEmails = await GetSentEmailsAsync(top: 1);

        // Then

        actualSentEmails.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsSentEmailById()
    {
        // Given
        SeededSentEmailContext seededContext = await SeedDatabase();
        string subject = Unique(prefix: "SentEmail");

        SentEmail expectedSentEmail = await CreateSentEmailAsync(payload: new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject,
            content = "Acceptance email content",
            to = "acceptance@example.com",
            cc = string.Empty,
            isBodyHtml = true,
            from = "acceptance@example.com",
            sentOn = DateTimeOffset.UtcNow,
        });

        SentEmail actualSentEmail;

        // When
        actualSentEmail = await GetSentEmailAsync(id: expectedSentEmail.Id);

        // Then

        actualSentEmail.Id.Should()
            .Be(expected: expectedSentEmail.Id);

        actualSentEmail.Subject.Should()
            .Be(expected: subject);

        await DeleteSentEmailAsync(id: expectedSentEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}