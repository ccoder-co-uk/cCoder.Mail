using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class Pop3MailReceiverServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveAsync()
    {
        // Given
        MailboxReceiveRequest request = new() { User = "mail@example.test" };
        string[][] rawMessages =
        [
            [
                "Message-ID: <message-1@example.test>",
                "From: sender@example.test",
                "To: mail@example.test",
                "Subject: Received",
                "Date: Tue, 30 Jun 2026 10:00:00 +0000",
                "Content-Type: text/plain",
                string.Empty,
                "Body",
            ],
        ];
        CancellationToken cancellationToken = new();

        pop3MailReceiverBrokerMock
            .Setup(broker => broker.ReceiveAsync(request, cancellationToken))
            .ReturnsAsync(rawMessages);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverService.ReceiveAsync(request, cancellationToken);

        // Then
        actualEmails.Should().ContainSingle();
        actualEmails[0].MessageId.Should().Be("<message-1@example.test>");
        actualEmails[0].From.Should().Be("sender@example.test");
        actualEmails[0].To.Should().Be("mail@example.test");
        actualEmails[0].Subject.Should().Be("Received");
        actualEmails[0].Content.Should().Be("Body");
        actualEmails[0].IsBodyHtml.Should().BeFalse();
        pop3MailReceiverBrokerMock.Verify(broker => broker.ReceiveAsync(request, cancellationToken), Times.Once);
        pop3MailReceiverBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveTopAsync()
    {
        // Given
        string[][] rawMessages =
        [
            [
                "Message-ID: <message-1@example.test>",
                "From: sender@example.test",
                "To: mail@example.test",
                "Subject: Received",
                "Date: Tue, 30 Jun 2026 10:00:00 +0000",
                "Content-Type: text/plain",
                string.Empty,
                "Body",
            ],
        ];
        CancellationToken cancellationToken = new();

        pop3MailReceiverBrokerMock
            .Setup(broker => broker.ReceiveTopAsync(1, cancellationToken))
            .ReturnsAsync(rawMessages);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverService.ReceiveTopAsync(1, cancellationToken);

        // Then
        actualEmails.Should().ContainSingle();
        actualEmails[0].MessageId.Should().Be("<message-1@example.test>");
        actualEmails[0].Subject.Should().Be("Received");
        actualEmails[0].Content.Should().Be("Body");
        pop3MailReceiverBrokerMock.Verify(broker => broker.ReceiveTopAsync(1, cancellationToken), Times.Once);
        pop3MailReceiverBrokerMock.VerifyNoOtherCalls();
    }
}
