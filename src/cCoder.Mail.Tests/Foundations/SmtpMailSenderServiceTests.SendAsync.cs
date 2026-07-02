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
        actualRequest.Host.Should().Be("smtp.example.test");
        actualRequest.Port.Should().Be(587);
        actualRequest.EnableSsl.Should().BeTrue();
        actualRequest.User.Should().Be("sender@example.test");
        actualRequest.Password.Should().Be("password");
        actualRequest.Message.From.Address.Should().Be("from@example.test");
        actualRequest.Message.To.Select(address => address.Address).Should().ContainSingle("to@example.test");
        actualRequest.Message.CC.Select(address => address.Address).Should().ContainSingle("cc@example.test");
        actualRequest.Message.Subject.Should().Be("Send");
        actualRequest.Message.Body.Should().Be("Body");
        actualRequest.Message.IsBodyHtml.Should().BeTrue();
        smtpMailSenderBrokerMock.Verify(
            broker => broker.SendAsync(
                It.IsAny<SmtpMailSendRequest>(),
                cancellationToken),
            Times.Once);
        smtpMailSenderBrokerMock.VerifyNoOtherCalls();
    }
}
