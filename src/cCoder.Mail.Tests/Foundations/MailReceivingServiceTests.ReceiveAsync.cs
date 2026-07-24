// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailReceivingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveAsync()
    {
        // Given
        MailboxReceiveRequest request = new()
        {
            Host = "mailbox.example.test",
            User = "user@example.test",
            Password = "password",
        };

        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        mailReceiverClientBrokerMock
            .Setup(expression: broker => broker.ReceiveAsync(request: request, cancellationToken: cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailReceivingService.ReceiveAsync(request: request, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        mailReceiverClientBrokerMock.Verify(expression: broker => broker.ReceiveAsync(request: request, cancellationToken: cancellationToken), times: Times.Once);
        mailReceiverClientBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveTopAsync()
    {
        // Given
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        mailReceiverClientBrokerMock
            .Setup(expression: broker => broker.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailReceivingService.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        mailReceiverClientBrokerMock.Verify(expression: broker => broker.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken), times: Times.Once);
        mailReceiverClientBrokerMock.VerifyNoOtherCalls();
    }
}