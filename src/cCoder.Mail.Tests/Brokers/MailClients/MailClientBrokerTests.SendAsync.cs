using cCoder.Data.Models.Mail;
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
}
