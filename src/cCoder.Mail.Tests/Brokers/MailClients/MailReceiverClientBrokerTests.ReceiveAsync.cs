// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
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
        Guid mailReceiverId = Guid.NewGuid();

        MailboxReceiveRequest request = new()
        {
            MailReceiverId = mailReceiverId,
            MaximumMessages = 12,
        };

        ReceivedEmail[] expectedEmails =
            [new() { Subject = "Received" }];

        CancellationToken cancellationToken = new();

        MailReceiverClientBroker mailReceiverClientBroker =
            new(mailClientFactory: mailClientFactoryMock.Object);

        mailClientFactoryMock
            .Setup(
                expression: factory =>
                    factory.CreateMailClientAsync(
                        mailReceiverId: mailReceiverId,
                        cancellationToken:
                            cancellationToken))
            .ReturnsAsync(value: mailClientMock.Object);

        mailClientMock
            .Setup(
                expression: client =>
                    client.ReceiveAsync(
                        mailReceiverId: mailReceiverId,
                        maximumMessages: 12,
                        cancellationToken:
                            cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails =
            await mailReceiverClientBroker.ReceiveAsync(
                request: request,
                cancellationToken: cancellationToken);

        // Then
        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        mailClientFactoryMock.VerifyAll();
        mailClientMock.VerifyAll();
        mailClientFactoryMock.VerifyNoOtherCalls();
        mailClientMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToReceiverWhenReceiveTopAsync()
    {
        // Given
        Guid mailReceiverId = Guid.NewGuid();

        ReceivedEmail[] expectedEmails =
            [new() { Subject = "Received" }];

        CancellationToken cancellationToken = new();

        MailReceiverClientBroker mailReceiverClientBroker =
            new(mailClientFactory: mailClientFactoryMock.Object);

        mailClientFactoryMock
            .Setup(
                expression: factory =>
                    factory.CreateMailClientAsync(
                        mailReceiverId: mailReceiverId,
                        cancellationToken:
                            cancellationToken))
            .ReturnsAsync(value: mailClientMock.Object);

        mailClientMock
            .Setup(
                expression: client =>
                    client.ReceiveAsync(
                        mailReceiverId: mailReceiverId,
                        maximumMessages: 1,
                        cancellationToken:
                            cancellationToken))
            .ReturnsAsync(value: expectedEmails);

        // When
        ReceivedEmail[] actualEmails =
            await mailReceiverClientBroker.ReceiveTopAsync(
                mailReceiverId: mailReceiverId,
                count: 1,
                cancellationToken: cancellationToken);

        // Then
        actualEmails.Should()
            .BeSameAs(expected: expectedEmails);

        mailClientFactoryMock.VerifyAll();
        mailClientMock.VerifyAll();
        mailClientFactoryMock.VerifyNoOtherCalls();
        mailClientMock.VerifyNoOtherCalls();
    }
}