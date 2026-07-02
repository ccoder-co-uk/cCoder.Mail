using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class Pop3MailReceiverServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveAsync()
    {
        // Given
        MailboxReceiveRequest request = new() { User = "mail@example.test" };
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        pop3MailReceiverBrokerMock
            .Setup(broker => broker.ReceiveAsync(request, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverService.ReceiveAsync(request, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        pop3MailReceiverBrokerMock.Verify(broker => broker.ReceiveAsync(request, cancellationToken), Times.Once);
        pop3MailReceiverBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveTopAsync()
    {
        // Given
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        pop3MailReceiverBrokerMock
            .Setup(broker => broker.ReceiveTopAsync(1, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverService.ReceiveTopAsync(1, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        pop3MailReceiverBrokerMock.Verify(broker => broker.ReceiveTopAsync(1, cancellationToken), Times.Once);
        pop3MailReceiverBrokerMock.VerifyNoOtherCalls();
    }
}
