// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .Setup(expression: broker => broker.OpenAsync(host: "pop3.example.test", port: 995, cancellationToken: cancellationToken))
            .ReturnsAsync(value: connection);

        pop3MailReceiverBrokerMock
            .SetupSequence(expression: broker => broker.ReadLineAsync(connection: connection, cancellationToken: cancellationToken))
            .ReturnsAsync(value: "+OK ready")
            .ReturnsAsync(value: "+OK user")
            .ReturnsAsync(value: "+OK pass")
            .ReturnsAsync(value: "+OK 1 100")
            .ReturnsAsync(value: "+OK message")
            .ReturnsAsync(value: "Message-ID: <message-1@example.test>")
            .ReturnsAsync(value: "From: sender@example.test")
            .ReturnsAsync(value: "To: mail@example.test")
            .ReturnsAsync(value: "Subject: Received")
            .ReturnsAsync(value: "Date: Tue, 30 Jun 2026 10:00:00 +0000")
            .ReturnsAsync(value: "Content-Type: text/plain")
            .ReturnsAsync(value: string.Empty)
            .ReturnsAsync(value: "Body")
            .ReturnsAsync(value: ".");

        pop3MailReceiverBrokerMock
            .Setup(expression: broker => broker.WriteLineAsync(connection: connection, line: It.IsAny<string>(), cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverService.ReceiveAsync(request: request, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .ContainSingle();

        actualEmails[0].MessageId.Should()
            .Be(expected: "<message-1@example.test>");

        actualEmails[0].From.Should()
            .Be(expected: "sender@example.test");

        actualEmails[0].To.Should()
            .Be(expected: "mail@example.test");

        actualEmails[0].Subject.Should()
            .Be(expected: "Received");

        actualEmails[0].Content.Should()
            .Be(expected: "Body");

        actualEmails[0].IsBodyHtml.Should()
            .BeFalse();

        pop3MailReceiverBrokerMock.Verify(expression: broker => broker.OpenAsync(host: "pop3.example.test", port: 995, cancellationToken: cancellationToken), times: Times.Once);

        pop3MailReceiverBrokerMock.Verify(
expression: broker => broker.WriteLineAsync(connection: connection, line: "USER mail@example.test", cancellationToken: cancellationToken),
times: Times.Once);

        pop3MailReceiverBrokerMock.Verify(
expression: broker => broker.WriteLineAsync(connection: connection, line: "PASS password", cancellationToken: cancellationToken),
times: Times.Once);

        pop3MailReceiverBrokerMock.Verify(
expression: broker => broker.WriteLineAsync(connection: connection, line: "STAT", cancellationToken: cancellationToken),
times: Times.Once);

        pop3MailReceiverBrokerMock.Verify(
expression: broker => broker.WriteLineAsync(connection: connection, line: "RETR 1", cancellationToken: cancellationToken),
times: Times.Once);

        pop3MailReceiverBrokerMock.Verify(
expression: broker => broker.WriteLineAsync(connection: connection, line: "QUIT", cancellationToken: cancellationToken),
times: Times.Once);

        pop3MailReceiverBrokerMock.Verify(
expression: broker => broker.ReadLineAsync(connection: connection, cancellationToken: cancellationToken),
times: Times.Exactly(callCount: 14));

        pop3MailReceiverBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveTopAsync()
    {
        // Given
        CancellationToken cancellationToken = new();
        MailClientTextConnection connection = new();
        mailConfiguration.Pop3.Host = "pop3.example.test";
        mailConfiguration.Pop3.User = "mail@example.test";
        mailConfiguration.Pop3.Password = "password";

        pop3MailReceiverBrokerMock
            .Setup(expression: broker => broker.OpenSslAsync(host: "pop3.example.test", port: 995, cancellationToken: cancellationToken))
            .ReturnsAsync(value: connection);

        pop3MailReceiverBrokerMock
            .SetupSequence(expression: broker => broker.ReadLineAsync(connection: connection, cancellationToken: cancellationToken))
            .ReturnsAsync(value: "+OK ready")
            .ReturnsAsync(value: "+OK user")
            .ReturnsAsync(value: "+OK pass")
            .ReturnsAsync(value: "+OK 1 100")
            .ReturnsAsync(value: "+OK message")
            .ReturnsAsync(value: "Message-ID: <message-1@example.test>")
            .ReturnsAsync(value: "From: sender@example.test")
            .ReturnsAsync(value: "To: mail@example.test")
            .ReturnsAsync(value: "Subject: Received")
            .ReturnsAsync(value: "Date: Tue, 30 Jun 2026 10:00:00 +0000")
            .ReturnsAsync(value: "Content-Type: text/plain")
            .ReturnsAsync(value: string.Empty)
            .ReturnsAsync(value: "Body")
            .ReturnsAsync(value: ".");

        pop3MailReceiverBrokerMock
            .Setup(expression: broker => broker.WriteLineAsync(connection: connection, line: It.IsAny<string>(), cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverService.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .ContainSingle();

        actualEmails[0].MessageId.Should()
            .Be(expected: "<message-1@example.test>");

        actualEmails[0].Subject.Should()
            .Be(expected: "Received");

        actualEmails[0].Content.Should()
            .Be(expected: "Body");

        pop3MailReceiverBrokerMock.Verify(expression: broker => broker.OpenSslAsync(host: "pop3.example.test", port: 995, cancellationToken: cancellationToken), times: Times.Once);

        pop3MailReceiverBrokerMock.Verify(
expression: broker => broker.WriteLineAsync(connection: connection, line: It.IsAny<string>(), cancellationToken: cancellationToken),
times: Times.Exactly(callCount: 5));

        pop3MailReceiverBrokerMock.Verify(
expression: broker => broker.ReadLineAsync(connection: connection, cancellationToken: cancellationToken),
times: Times.Exactly(callCount: 14));

        pop3MailReceiverBrokerMock.VerifyNoOtherCalls();
    }
}