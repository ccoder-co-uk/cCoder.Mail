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

        mailClientBrokerMock
            .Setup(broker => broker.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await mailSendingService.SendAsync(email, cancellationToken);

        // Then
        mailClientBrokerMock.Verify(broker => broker.SendAsync(email, cancellationToken), Times.Once);
        mailClientBrokerMock.VerifyNoOtherCalls();
    }
}
