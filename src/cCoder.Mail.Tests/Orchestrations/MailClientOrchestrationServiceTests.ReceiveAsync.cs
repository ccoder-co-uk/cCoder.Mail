using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class MailClientOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToReceivingServiceWhenReceiveAsync()
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

        mailReceivingServiceMock
            .Setup(service => service.ReceiveAsync(request, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailClientOrchestrationService.ReceiveAsync(request, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        mailReceivingServiceMock.Verify(service => service.ReceiveAsync(request, cancellationToken), Times.Once);
        mailReceivingServiceMock.VerifyNoOtherCalls();
        mailSendingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToReceivingServiceWhenReceiveTopAsync()
    {
        // Given
        ReceivedEmail[] expectedEmails = [new() { Subject = "Received" }];
        CancellationToken cancellationToken = new();

        mailReceivingServiceMock
            .Setup(service => service.ReceiveTopAsync(1, cancellationToken))
            .ReturnsAsync(expectedEmails);

        // When
        ReceivedEmail[] actualEmails = await mailClientOrchestrationService.ReceiveTopAsync(1, cancellationToken);

        // Then
        actualEmails.Should().BeSameAs(expectedEmails);
        mailReceivingServiceMock.Verify(service => service.ReceiveTopAsync(1, cancellationToken), Times.Once);
        mailReceivingServiceMock.VerifyNoOtherCalls();
        mailSendingServiceMock.VerifyNoOtherCalls();
    }
}
