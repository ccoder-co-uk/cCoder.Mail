// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class Pop3MailReceiverProviderTests
{
    [Fact]
    public async Task ShouldDelegateToServiceWhenReceiveAsync()
    {
        // Given
        MailboxReceiveRequest request = new() { User = "mail@example.test" };
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        pop3MailReceiverServiceMock
            .Setup(expression: service => service.ReceiveAsync(request: request, cancellationToken: cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverProvider.ReceiveAsync(request: request, cancellationToken: cancellationToken);

        // Then

        pop3MailReceiverProvider.ProviderName.Should()
            .Be(expected: MailProviderNames.Pop3);

        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        pop3MailReceiverServiceMock.Verify(expression: service => service.ReceiveAsync(request: request, cancellationToken: cancellationToken), times: Times.Once);
        pop3MailReceiverServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToServiceWhenReceiveTopAsync()
    {
        // Given
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        pop3MailReceiverServiceMock
            .Setup(expression: service => service.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverProvider.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        pop3MailReceiverServiceMock.Verify(expression: service => service.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken), times: Times.Once);
        pop3MailReceiverServiceMock.VerifyNoOtherCalls();
    }
}