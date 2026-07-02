using cCoder.Data.Models.Mail;
using FizzWare.NBuilder;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class MailClientOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToSendingServiceWhenSendAsync()
    {
        // Given
        QueuedEmail email = Builder<QueuedEmail>.CreateNew().Build();
        CancellationToken cancellationToken = new();

        mailSendingServiceMock
            .Setup(service => service.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await mailClientOrchestrationService.SendAsync(email, cancellationToken);

        // Then
        mailSendingServiceMock.Verify(service => service.SendAsync(email, cancellationToken), Times.Once);
        mailSendingServiceMock.VerifyNoOtherCalls();
        mailReceivingServiceMock.VerifyNoOtherCalls();
    }
}
