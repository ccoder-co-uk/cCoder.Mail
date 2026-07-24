// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class MailClientOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToReceivingServiceWhenReceiveAsync()
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

        mailReceivingServiceMock
            .Setup(expression: service => service.ReceiveAsync(request: request, cancellationToken: cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailClientOrchestrationService.ReceiveAsync(request: request, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        mailReceivingServiceMock.Verify(expression: service => service.ReceiveAsync(request: request, cancellationToken: cancellationToken), times: Times.Once);
        mailReceivingServiceMock.VerifyNoOtherCalls();
        mailSendingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToReceivingServiceWhenReceiveTopAsync()
    {
        // Given
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        mailReceivingServiceMock
            .Setup(expression: service => service.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailClientOrchestrationService.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        mailReceivingServiceMock.Verify(expression: service => service.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken), times: Times.Once);
        mailReceivingServiceMock.VerifyNoOtherCalls();
        mailSendingServiceMock.VerifyNoOtherCalls();
    }
}