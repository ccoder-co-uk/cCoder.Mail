using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailReceivingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveAsync()
    {
        // Given
        MailboxReceiveRequest request = new()
        {
            Host = "mailbox.example.test",
            User = "user@example.test",
            Password = "password",
        };
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        mailClientBrokerMock
            .Setup(broker => broker.ReceiveAsync(request, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailReceivingService.ReceiveAsync(request, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        mailClientBrokerMock.Verify(broker => broker.ReceiveAsync(request, cancellationToken), Times.Once);
        mailClientBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToBrokerWhenReceiveTopAsync()
    {
        // Given
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        mailClientBrokerMock
            .Setup(broker => broker.ReceiveTopAsync(1, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailReceivingService.ReceiveTopAsync(1, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        mailClientBrokerMock.Verify(broker => broker.ReceiveTopAsync(1, cancellationToken), Times.Once);
        mailClientBrokerMock.VerifyNoOtherCalls();
    }
}
