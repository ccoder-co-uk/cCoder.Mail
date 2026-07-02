using cCoder.Mail.Models;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class ReceivedEmailControllerTests
{
    [Fact]
    public async Task ReceiveTop_ReturnsTopEmailsFromConfiguredMailbox()
    {
        // When
        ReceivedEmail[] receivedEmails = await ReceiveTopEmailsAsync(1);

        // Then
        receivedEmails.Should().HaveCount(1);
        receivedEmails[0].MessageId.Should().Be("<acceptance-top-message@example.test>");
        receivedEmails[0].From.Should().Be("configured@example.test");
        receivedEmails[0].Subject.Should().Be("Acceptance top 1");
        receivedEmails[0].Content.Should().Be("Acceptance top receive content");
    }
}
