// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSenderServiceTests
{
    [Fact]
    public async Task DeleteAllMailSenderAsyncShouldDelegateToBrokerAsync()
    {
        MailSender[] senders = [CreateRandomMailSender()];
        mailSenderBrokerMock.Setup(expression: broker =>
                broker.DeleteAllMailSendersAsync(senders))
            .Returns(value: ValueTask.CompletedTask);

        await mailSenderService.DeleteAllMailSenderAsync(deletedMailSender: senders);

        mailSenderBrokerMock.Verify(expression: broker =>
            broker.DeleteAllMailSendersAsync(senders), Times.Once);
    }

    [Fact]
    public async Task DeleteAllByAppIdAsyncShouldDelegateToBrokerAsync()
    {
        mailSenderBrokerMock.Setup(expression: broker =>
                broker.DeleteAllMailSendersByAppIdAsync(7))
            .Returns(value: ValueTask.CompletedTask);

        await mailSenderService.DeleteAllByAppIdAsync(appId: 7);

        mailSenderBrokerMock.Verify(expression: broker =>
            broker.DeleteAllMailSendersByAppIdAsync(7), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005