// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class ReceivedEmailControllerTests
{
    [Fact]
    public async Task ReceiveTop_ReturnsTopEmailsFromConfiguredMailbox()
    {
        // Given

        // When
        ReceivedEmail[] receivedEmails = await ReceiveTopEmailsAsync(count: 1);

        // Then

        receivedEmails.Should()
            .HaveCount(expected: 1);

        receivedEmails[0].MessageId.Should()
            .Be(expected: "<acceptance-top-message@example.test>");

        receivedEmails[0].From.Should()
            .Be(expected: "configured@example.test");

        receivedEmails[0].Subject.Should()
            .Be(expected: "Acceptance top 1");

        receivedEmails[0].Content.Should()
            .Be(expected: "Acceptance top receive content");
    }
}