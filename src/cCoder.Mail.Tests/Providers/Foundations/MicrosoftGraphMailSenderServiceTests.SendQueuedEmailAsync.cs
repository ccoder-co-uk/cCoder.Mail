// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers.Foundations;

public partial class MicrosoftGraphMailSenderServiceTests
{
    [Fact]
    public async Task SendQueuedEmailAsyncShouldSendThroughGraphAsync()
    {
        QueuedEmail email = CreateEmail();
        graphBrokerMock.Setup(expression: broker => broker.SendEmailAsync(
                email,
                configuration,
                CancellationToken.None))
            .ReturnsAsync(value: new HttpClientBrokerResponse(true, string.Empty));

        await service.SendQueuedEmailAsync(email);

        graphBrokerMock.Verify(expression: broker => broker.SendEmailAsync(
            email,
            configuration,
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SendQueuedEmailAsyncShouldRejectGraphFailureAsync()
    {
        QueuedEmail email = CreateEmail();
        graphBrokerMock.Setup(expression: broker => broker.SendEmailAsync(
                email,
                configuration,
                CancellationToken.None))
            .ReturnsAsync(value: new HttpClientBrokerResponse(false, "failure"));

        Func<Task> action = async () => await service.SendQueuedEmailAsync(email);

        await action.Should().ThrowAsync<Exception>();
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task SendQueuedEmailAsyncShouldRejectIncompleteMessageAsync(
        bool hasSender,
        bool hasRecipient)
    {
        QueuedEmail email = CreateEmail();
        email.MailSender = hasSender ? email.MailSender : null;
        email.To = hasRecipient ? email.To : null;

        Func<Task> action = async () => await service.SendQueuedEmailAsync(email);

        await action.Should().ThrowAsync<Exception>();
    }

    private static QueuedEmail CreateEmail() =>
        new()
        {
            To = "to@example.test",
            Subject = "Subject",
            Content = "Body",
            MailSender = new MailSender { User = "sender@example.test" }
        };
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005