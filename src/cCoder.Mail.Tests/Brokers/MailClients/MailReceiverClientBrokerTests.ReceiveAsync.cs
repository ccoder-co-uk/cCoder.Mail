// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailReceiverClientBrokerTests
{
    [Fact]
    public async Task ShouldDelegateToConfiguredReceiverWhenReceiveAsync()
    {
        // Given
        MailboxReceiveRequest request = new()
        {
            ProviderName = "MicrosoftGraph",
            User = "mailbox@example.test",
        };

        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        mailReceiverFactoryMock
            .Setup(expression: factory => factory.GetReceiver(providerName: "MicrosoftGraph"))
            .Returns(value: mailReceiverProviderMock.Object);

        mailReceiverProviderMock
            .Setup(expression: provider => provider.ReceiveAsync(request: request, cancellationToken: cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailReceiverClientBroker.ReceiveAsync(request: request, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        mailReceiverFactoryMock.Verify(expression: factory => factory.GetReceiver(providerName: "MicrosoftGraph"), times: Times.Once);
        mailReceiverProviderMock.Verify(expression: provider => provider.ReceiveAsync(request: request, cancellationToken: cancellationToken), times: Times.Once);
        mailReceiverFactoryMock.VerifyNoOtherCalls();
        mailReceiverProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToDefaultReceiverWhenReceiveTopAsync()
    {
        // Given
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        mailReceiverFactoryMock
            .Setup(expression: factory => factory.GetReceiver(providerName: null))
            .Returns(value: mailReceiverProviderMock.Object);

        mailReceiverProviderMock
            .Setup(expression: provider => provider.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailReceiverClientBroker.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken);

        // Then

        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        mailReceiverFactoryMock.Verify(expression: factory => factory.GetReceiver(providerName: null), times: Times.Once);
        mailReceiverProviderMock.Verify(expression: provider => provider.ReceiveTopAsync(count: 1, cancellationToken: cancellationToken), times: Times.Once);
        mailReceiverFactoryMock.VerifyNoOtherCalls();
        mailReceiverProviderMock.VerifyNoOtherCalls();
    }
}