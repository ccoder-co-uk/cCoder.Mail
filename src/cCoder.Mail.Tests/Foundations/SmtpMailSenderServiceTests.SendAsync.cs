// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        smtpMailSenderBrokerMock
            .Setup(
                expression: broker =>
                    broker.SendAsync(
                        email: email,
                        cancellationToken: cancellationToken))
            .Returns(value: Task.CompletedTask);

        // When
        await smtpMailSenderService.SendQueuedEmailAsync(email: email, cancellationToken: cancellationToken);

        // Then
        smtpMailSenderBrokerMock.Verify(
            expression: broker =>
                broker.SendAsync(
                    email: email,
                    cancellationToken: cancellationToken),
            times: Times.Once);

        smtpMailSenderBrokerMock.VerifyNoOtherCalls();
    }
}