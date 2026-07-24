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
    public async Task Patch_UpdatesSentEmail()
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

        string updatedSubject = Unique(prefix: "PatchedSentEmail");
        SentEmail actualSentEmail;

        // When

        await PatchSentEmailAsync(id: createdSentEmail.Id, payload: new
        {
            subject = updatedSubject,
            cc = "patched@example.com",
        });

        actualSentEmail = await GetSentEmailAsync(id: createdSentEmail.Id);

        // Then

        actualSentEmail.Subject.Should()
            .Be(expected: updatedSubject);

        actualSentEmail.CC.Should()
            .Be(expected: "patched@example.com");

        await DeleteSentEmailAsync(id: createdSentEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}