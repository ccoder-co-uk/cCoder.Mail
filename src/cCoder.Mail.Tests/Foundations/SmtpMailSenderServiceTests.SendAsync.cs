// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .Setup(expression: broker => broker.SendAsync(
request: It.IsAny<SmtpMailSendRequest>(),
cancellationToken: cancellationToken))
            .Callback<SmtpMailSendRequest, CancellationToken>(action: (request, _) => actualRequest = request)
            .Returns(value: Task.CompletedTask);

        // When
        await smtpMailSenderService.SendQueuedEmailAsync(email: email, cancellationToken: cancellationToken);

        // Then

        actualRequest.Host.Should()
            .Be(expected: "smtp.example.test");

        actualRequest.Port.Should()
            .Be(expected: 587);

        actualRequest.EnableSsl.Should()
            .BeTrue();

        actualRequest.User.Should()
            .Be(expected: "sender@example.test");

        actualRequest.Password.Should()
            .Be(expected: "password");

        actualRequest.Message.From.Address.Should()
            .Be(expected: "from@example.test");

        actualRequest.Message.To.Select(selector: address => address.Address)
            .Should()
            .ContainSingle(because: "to@example.test");

        actualRequest.Message.CC.Select(selector: address => address.Address)
            .Should()
            .ContainSingle(because: "cc@example.test");

        actualRequest.Message.Subject.Should()
            .Be(expected: "Send");

        actualRequest.Message.Body.Should()
            .Be(expected: "Body");

        actualRequest.Message.IsBodyHtml.Should()
            .BeTrue();

        smtpMailSenderBrokerMock.Verify(
expression: broker => broker.SendAsync(
request: It.IsAny<SmtpMailSendRequest>(),
cancellationToken: cancellationToken),
times: Times.Once);

        smtpMailSenderBrokerMock.VerifyNoOtherCalls();
    }
}