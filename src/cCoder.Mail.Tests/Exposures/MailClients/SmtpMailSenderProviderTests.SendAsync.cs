using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Exposures.MailClients;

public partial class SmtpMailSenderProviderTests
{
    [Fact]
    public async Task ShouldDelegateToServiceWhenSendAsync()
    {
        // Given
        QueuedEmail email = new() { Subject = "Send" };
        CancellationToken cancellationToken = new();

        smtpMailSenderServiceMock
            .Setup(service => service.SendAsync(email, cancellationToken))
            .Returns(Task.CompletedTask);

        // When
        await smtpMailSenderProvider.SendAsync(email, cancellationToken);

        // Then
        smtpMailSenderProvider.ProviderName.Should().Be(MailProviderNames.Smtp);
        smtpMailSenderServiceMock.Verify(service => service.SendAsync(email, cancellationToken), Times.Once);
        smtpMailSenderServiceMock.VerifyNoOtherCalls();
    }
}
