using cCoder.Data.Models.Mail;
using FizzWare.NBuilder;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenSendAsync()
    {
        // Given
        QueuedEmail email = Builder<QueuedEmail>.CreateNew().Build();
        CancellationToken cancellationToken = new();

        mailSenderClientBrokerMock
            .Setup(broker => broker.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await mailSendingService.SendAsync(email, cancellationToken);

        // Then
        mailSenderClientBrokerMock.Verify(broker => broker.SendAsync(email, cancellationToken), Times.Once);
        mailSenderClientBrokerMock.VerifyNoOtherCalls();
    }
}
