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
    public async Task Put_UpdatesSentEmail()
    {
        // Given
        SeededSentEmailContext seededContext = await SeedDatabase();

        SentEmail createdSentEmail = await CreateSentEmailAsync(payload: new
        {
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = Unique(prefix: "SentEmail"),
            content = "Acceptance email content",
            to = "acceptance@example.com",
            cc = string.Empty,
            isBodyHtml = true,
            from = "acceptance@example.com",
            sentOn = DateTimeOffset.UtcNow,
        });

        string updatedSubject = Unique(prefix: "UpdatedSentEmail");
        SentEmail actualSentEmail;

        // When

        await UpdateSentEmailAsync(id: createdSentEmail.Id, payload: new
        {
            id = createdSentEmail.Id,
            appId = seededContext.AppId,
            sentByUserId = "Guest",
            subject = updatedSubject,
            content = "Updated email content",
            to = "acceptance@example.com",
            cc = "cc@example.com",
            isBodyHtml = true,
            from = "acceptance@example.com",
            sentOn = DateTimeOffset.UtcNow,
        });

        actualSentEmail = await GetSentEmailAsync(id: createdSentEmail.Id);

        // Then

        actualSentEmail.Subject.Should()
            .Be(expected: updatedSubject);

        await DeleteSentEmailAsync(id: createdSentEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}