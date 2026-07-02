using cCoder.Data.Models.Mail;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailClientBrokerTests
{
    [Fact]
    public async Task ShouldDelegateToMailClientWhenSendAsync()
    {
        // Given
        QueuedEmail email = new() { Subject = "Send" };
        CancellationToken cancellationToken = new();

        mailClientMock
            .Setup(client => client.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await mailClientBroker.SendAsync(email, cancellationToken);

        // Then
        mailClientMock.Verify(client => client.SendAsync(email, cancellationToken), Times.Once);
        mailClientMock.VerifyNoOtherCalls();
        microsoftGraphClientMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToMicrosoftGraphClientWhenSendAsync()
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

        microsoftGraphClientMock
            .Setup(client => client.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await mailClientBroker.SendAsync(email, cancellationToken);

        // Then
        microsoftGraphClientMock.Verify(client => client.SendAsync(email, cancellationToken), Times.Once);
        microsoftGraphClientMock.VerifyNoOtherCalls();
        mailClientMock.VerifyNoOtherCalls();
    }
}
