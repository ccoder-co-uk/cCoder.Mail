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
        MailboxReceiveRequest request = new()
        {
            Host = "pop3.example.test",
            Port = 995,
            EnableSSL = false,
            User = "mail@example.test",
            Password = "password",
            MaximumMessages = 1,
        };
        CancellationToken cancellationToken = new();
        MailClientTextConnection connection = new();

        pop3MailReceiverBrokerMock
            .Setup(broker => broker.OpenAsync("pop3.example.test", 995, cancellationToken))
            .ReturnsAsync(connection);
        pop3MailReceiverBrokerMock
            .SetupSequence(broker => broker.ReadLineAsync(connection, cancellationToken))
            .ReturnsAsync("+OK ready")
            .ReturnsAsync("+OK user")
            .ReturnsAsync("+OK pass")
            .ReturnsAsync("+OK 1 100")
            .ReturnsAsync("+OK message")
            .ReturnsAsync("Message-ID: <message-1@example.test>")
            .ReturnsAsync("From: sender@example.test")
            .ReturnsAsync("To: mail@example.test")
            .ReturnsAsync("Subject: Received")
            .ReturnsAsync("Date: Tue, 30 Jun 2026 10:00:00 +0000")
            .ReturnsAsync("Content-Type: text/plain")
            .ReturnsAsync(string.Empty)
            .ReturnsAsync("Body")
            .ReturnsAsync(".");
        pop3MailReceiverBrokerMock
            .Setup(broker => broker.WriteLineAsync(connection, It.IsAny<string>(), cancellationToken))
            .Returns(Task.CompletedTask);

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
        pop3MailReceiverBrokerMock.Verify(broker => broker.OpenAsync("pop3.example.test", 995, cancellationToken), Times.Once);
        pop3MailReceiverBrokerMock.Verify(
            broker => broker.WriteLineAsync(connection, "USER mail@example.test", cancellationToken),
            Times.Once);
        pop3MailReceiverBrokerMock.Verify(
            broker => broker.WriteLineAsync(connection, "PASS password", cancellationToken),
            Times.Once);
        pop3MailReceiverBrokerMock.Verify(
            broker => broker.WriteLineAsync(connection, "STAT", cancellationToken),
            Times.Once);
        pop3MailReceiverBrokerMock.Verify(
            broker => broker.WriteLineAsync(connection, "RETR 1", cancellationToken),
            Times.Once);
        pop3MailReceiverBrokerMock.Verify(
            broker => broker.WriteLineAsync(connection, "QUIT", cancellationToken),
            Times.Once);
        pop3MailReceiverBrokerMock.Verify(
            broker => broker.ReadLineAsync(connection, cancellationToken),
            Times.Exactly(14));
        pop3MailReceiverBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveTopAsync()
    {
        // Given
        CancellationToken cancellationToken = new();
        MailClientTextConnection connection = new();
        Environment.SetEnvironmentVariable("CCODER_MAIL_RECEIVE_HOST", "pop3.example.test");
        Environment.SetEnvironmentVariable("CCODER_MAIL_RECEIVE_USER", "mail@example.test");
        Environment.SetEnvironmentVariable("CCODER_MAIL_RECEIVE_PASSWORD", "password");

        pop3MailReceiverBrokerMock
            .Setup(broker => broker.OpenSslAsync("pop3.example.test", 995, cancellationToken))
            .ReturnsAsync(connection);
        pop3MailReceiverBrokerMock
            .SetupSequence(broker => broker.ReadLineAsync(connection, cancellationToken))
            .ReturnsAsync("+OK ready")
            .ReturnsAsync("+OK user")
            .ReturnsAsync("+OK pass")
            .ReturnsAsync("+OK 1 100")
            .ReturnsAsync("+OK message")
            .ReturnsAsync("Message-ID: <message-1@example.test>")
            .ReturnsAsync("From: sender@example.test")
            .ReturnsAsync("To: mail@example.test")
            .ReturnsAsync("Subject: Received")
            .ReturnsAsync("Date: Tue, 30 Jun 2026 10:00:00 +0000")
            .ReturnsAsync("Content-Type: text/plain")
            .ReturnsAsync(string.Empty)
            .ReturnsAsync("Body")
            .ReturnsAsync(".");
        pop3MailReceiverBrokerMock
            .Setup(broker => broker.WriteLineAsync(connection, It.IsAny<string>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        ReceivedEmail[] actualEmails;

        try
        {
            actualEmails = await pop3MailReceiverService.ReceiveTopAsync(1, cancellationToken);
        }
        finally
        {
            Environment.SetEnvironmentVariable("CCODER_MAIL_RECEIVE_HOST", null);
            Environment.SetEnvironmentVariable("CCODER_MAIL_RECEIVE_USER", null);
            Environment.SetEnvironmentVariable("CCODER_MAIL_RECEIVE_PASSWORD", null);
        }

        // Then
        actualEmails.Should().ContainSingle();
        actualEmails[0].MessageId.Should().Be("<message-1@example.test>");
        actualEmails[0].Subject.Should().Be("Received");
        actualEmails[0].Content.Should().Be("Body");
        pop3MailReceiverBrokerMock.Verify(broker => broker.OpenSslAsync("pop3.example.test", 995, cancellationToken), Times.Once);
        pop3MailReceiverBrokerMock.Verify(
            broker => broker.WriteLineAsync(connection, It.IsAny<string>(), cancellationToken),
            Times.Exactly(5));
        pop3MailReceiverBrokerMock.Verify(
            broker => broker.ReadLineAsync(connection, cancellationToken),
            Times.Exactly(14));
        pop3MailReceiverBrokerMock.VerifyNoOtherCalls();
    }
}
