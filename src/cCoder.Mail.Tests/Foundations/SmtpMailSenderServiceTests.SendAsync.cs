using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class SmtpMailSenderServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenSendAsync()
    {
        // Given
        QueuedEmail email = new()
        {
            To = "to@example.test",
            CC = "cc@example.test",
            Subject = "Send",
            Content = "Body",
            IsBodyHtml = true,
            MailSender = new MailSender
            {
                Host = "smtp.example.test",
                Port = 587,
                EnableSSL = true,
                User = "sender@example.test",
                Password = "password",
                FromEmail = "from@example.test",
            },
        };
        CancellationToken cancellationToken = new();
        SmtpMailSendRequest actualRequest = null;

        smtpMailSenderBrokerMock
            .Setup(broker => broker.SendAsync(
                It.IsAny<SmtpMailSendRequest>(),
                cancellationToken))
            .Callback<SmtpMailSendRequest, CancellationToken>((request, _) => actualRequest = request)
            .Returns(Task.CompletedTask);

        // When
        await smtpMailSenderService.SendAsync(email, cancellationToken);

        // Then
        actualRequest.Should().BeEquivalentTo(new SmtpMailSendRequest
        {
            Host = "smtp.example.test",
            Port = 587,
            EnableSsl = true,
            User = "sender@example.test",
            Password = "password",
            From = "from@example.test",
            To = "to@example.test",
            CC = "cc@example.test",
            Subject = "Send",
            Content = "Body",
            IsBodyHtml = true,
        });
        smtpMailSenderBrokerMock.Verify(
            broker => broker.SendAsync(
                It.IsAny<SmtpMailSendRequest>(),
                cancellationToken),
            Times.Once);
        smtpMailSenderBrokerMock.VerifyNoOtherCalls();
    }
}
