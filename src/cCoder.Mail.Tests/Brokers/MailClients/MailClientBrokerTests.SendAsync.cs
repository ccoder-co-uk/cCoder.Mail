using cCoder.Data.Models.Mail;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailClientBrokerTests
{
    [Fact]
    public async Task ShouldDelegateToDefaultSenderWhenSendAsync()
    {
        // Given
        QueuedEmail email = new() { Subject = "Send" };
        CancellationToken cancellationToken = new();

        mailSenderFactoryMock
            .Setup(factory => factory.GetSender(null))
            .Returns(mailSenderProviderMock.Object);
        mailSenderProviderMock
            .Setup(provider => provider.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await mailClientBroker.SendAsync(email, cancellationToken);

        // Then
        mailSenderFactoryMock.Verify(factory => factory.GetSender(null), Times.Once);
        mailSenderProviderMock.Verify(provider => provider.SendAsync(email, cancellationToken), Times.Once);
        mailSenderFactoryMock.VerifyNoOtherCalls();
        mailSenderProviderMock.VerifyNoOtherCalls();
        mailReceiverFactoryMock.VerifyNoOtherCalls();
        mailReceiverProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToProviderSelectedByMailServerHostWhenSendAsync()
    {
        // Given
        QueuedEmail email = new()
        {
            MailServerName = "Graph",
            Subject = "Send",
            App = new App
            {
                MailServers =
                [
                    new()
                    {
                        Name = "Graph",
                        Host = "graph.microsoft.com",
                    }
                ],
            },
        };
        CancellationToken cancellationToken = new();

        mailSenderFactoryMock
            .Setup(factory => factory.GetSender("graph.microsoft.com"))
            .Returns(mailSenderProviderMock.Object);
        mailSenderProviderMock
            .Setup(provider => provider.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await mailClientBroker.SendAsync(email, cancellationToken);

        // Then
        mailSenderFactoryMock.Verify(factory => factory.GetSender("graph.microsoft.com"), Times.Once);
        mailSenderProviderMock.Verify(provider => provider.SendAsync(email, cancellationToken), Times.Once);
        mailSenderFactoryMock.VerifyNoOtherCalls();
        mailSenderProviderMock.VerifyNoOtherCalls();
        mailReceiverFactoryMock.VerifyNoOtherCalls();
        mailReceiverProviderMock.VerifyNoOtherCalls();
    }
}
