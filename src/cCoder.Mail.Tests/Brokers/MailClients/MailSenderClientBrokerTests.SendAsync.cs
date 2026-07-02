using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailSenderClientBrokerTests
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
        await mailSenderClientBroker.SendAsync(email, cancellationToken);

        // Then
        mailSenderFactoryMock.Verify(factory => factory.GetSender(null), Times.Once);
        mailSenderProviderMock.Verify(provider => provider.SendAsync(email, cancellationToken), Times.Once);
        mailSenderFactoryMock.VerifyNoOtherCalls();
        mailSenderProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToProviderSelectedByMailSenderProviderNameWhenSendAsync()
    {
        // Given
        QueuedEmail email = new()
        {
            MailServerName = "Graph",
            Subject = "Send",
            App = new App
            {
                MailSenders =
                [
                    new()
                    {
                        Name = "Graph",
                        ProviderName = "MicrosoftGraph",
                    }
                ],
            },
        };
        CancellationToken cancellationToken = new();

        mailSenderFactoryMock
            .Setup(factory => factory.GetSender("MicrosoftGraph"))
            .Returns(mailSenderProviderMock.Object);
        mailSenderProviderMock
            .Setup(provider => provider.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await mailSenderClientBroker.SendAsync(email, cancellationToken);

        // Then
        mailSenderFactoryMock.Verify(factory => factory.GetSender("MicrosoftGraph"), Times.Once);
        mailSenderProviderMock.Verify(provider => provider.SendAsync(email, cancellationToken), Times.Once);
        mailSenderFactoryMock.VerifyNoOtherCalls();
        mailSenderProviderMock.VerifyNoOtherCalls();
    }
}
