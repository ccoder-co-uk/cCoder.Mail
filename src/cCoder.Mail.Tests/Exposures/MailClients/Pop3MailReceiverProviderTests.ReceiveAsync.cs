using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class Pop3MailReceiverProviderTests
{
    [Fact]
    public async Task ShouldDelegateToServiceWhenReceiveAsync()
    {
        // Given
        MailboxReceiveRequest request = new() { User = "mail@example.test" };
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        pop3MailReceiverServiceMock
            .Setup(service => service.ReceiveAsync(request, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverProvider.ReceiveAsync(request, cancellationToken);

        // Then
        pop3MailReceiverProvider.ProviderName.Should().Be(MailProviderNames.Pop3);
        actualEmails.Should().BeSameAs(expectedEmails);
        pop3MailReceiverServiceMock.Verify(service => service.ReceiveAsync(request, cancellationToken), Times.Once);
        pop3MailReceiverServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToServiceWhenReceiveTopAsync()
    {
        // Given
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        pop3MailReceiverServiceMock
            .Setup(service => service.ReceiveTopAsync(1, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await pop3MailReceiverProvider.ReceiveTopAsync(1, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        pop3MailReceiverServiceMock.Verify(service => service.ReceiveTopAsync(1, cancellationToken), Times.Once);
        pop3MailReceiverServiceMock.VerifyNoOtherCalls();
    }
}
