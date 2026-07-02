using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.Mail;

public sealed partial class ReceivedEmailControllerTests
{
    [Fact]
    public async Task Receive_ReturnsReceivedEmailsFromConfiguredProvider()
    {
        // Given
        DateTimeOffset from = DateTimeOffset.UtcNow.AddMinutes(-5);

        // When
        ReceivedEmail[] receivedEmails = await ReceiveEmailsAsync(new
        {
            user = "sender@example.test",
            from,
            to = DateTimeOffset.UtcNow.AddMinutes(5),
            maximumMessages = 10,
        });

        // Then
        receivedEmails.Should().HaveCount(1);
        receivedEmails[0].MessageId.Should().Be("<acceptance-message@example.test>");
        receivedEmails[0].From.Should().Be("sender@example.test");
        receivedEmails[0].Subject.Should().Be("Acceptance receive from sender@example.test");
        receivedEmails[0].Content.Should().Be("Acceptance receive content");
    }
}
