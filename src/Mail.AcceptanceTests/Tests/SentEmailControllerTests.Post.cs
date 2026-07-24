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
    public async Task Post_CreatesSentEmail()
    {
        // Given
        SeededSentEmailContext seededContext = await SeedDatabase();
        string subject = Unique(prefix: "SentEmail");
        SentEmail expectedSentEmail;
        SentEmail actualSentEmail;

        // When

        expectedSentEmail = await CreateSentEmailAsync(payload: new
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

        actualSentEmail = await GetSentEmailAsync(id: expectedSentEmail.Id);

        // Then

        actualSentEmail.Subject.Should()
            .Be(expected: subject);

        await DeleteSentEmailAsync(id: expectedSentEmail.Id);
        await Teardown(seededContext: seededContext);
    }
}