using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Brokers.MailClients;

public partial class MailClientBrokerTests
{
    [Fact]
    public async Task ShouldDelegateToMicrosoftGraphClientWhenReceiveAsync()
    {
        // Given
        MailboxReceiveRequest request = new() { User = "mailbox@example.test" };
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        microsoftGraphClientMock
            .Setup(client => client.ReceiveAsync(request, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailClientBroker.ReceiveAsync(request, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        microsoftGraphClientMock.Verify(client => client.ReceiveAsync(request, cancellationToken), Times.Once);
        microsoftGraphClientMock.VerifyNoOtherCalls();
        mailClientMock.VerifyNoOtherCalls();
    }
}
