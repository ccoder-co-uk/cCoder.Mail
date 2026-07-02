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
            .Setup(factory => factory.GetReceiver("MicrosoftGraph"))
            .Returns(mailReceiverProviderMock.Object);
        mailReceiverProviderMock
            .Setup(provider => provider.ReceiveAsync(request, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailReceiverClientBroker.ReceiveAsync(request, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        mailReceiverFactoryMock.Verify(factory => factory.GetReceiver("MicrosoftGraph"), Times.Once);
        mailReceiverProviderMock.Verify(provider => provider.ReceiveAsync(request, cancellationToken), Times.Once);
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
            .Setup(factory => factory.GetReceiver(null))
            .Returns(mailReceiverProviderMock.Object);
        mailReceiverProviderMock
            .Setup(provider => provider.ReceiveTopAsync(1, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailReceiverClientBroker.ReceiveTopAsync(1, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        mailReceiverFactoryMock.Verify(factory => factory.GetReceiver(null), Times.Once);
        mailReceiverProviderMock.Verify(provider => provider.ReceiveTopAsync(1, cancellationToken), Times.Once);
        mailReceiverFactoryMock.VerifyNoOtherCalls();
        mailReceiverProviderMock.VerifyNoOtherCalls();
    }
}
