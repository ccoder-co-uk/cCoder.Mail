using cCoder.Data.Models.Mail;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class SmtpMailSenderServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenSendAsync()
    {
        // Given
        QueuedEmail email = new() { Subject = "Send" };
        CancellationToken cancellationToken = new();

        smtpMailSenderBrokerMock
            .Setup(broker => broker.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await smtpMailSenderService.SendAsync(email, cancellationToken);

        // Then
        smtpMailSenderBrokerMock.Verify(broker => broker.SendAsync(email, cancellationToken), Times.Once);
        smtpMailSenderBrokerMock.VerifyNoOtherCalls();
    }
}
