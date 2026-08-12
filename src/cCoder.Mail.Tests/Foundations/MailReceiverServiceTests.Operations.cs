// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailReceiverServiceTests
{
    [Fact]
    public void GetEnabledShouldReturnEnabledReceivers()
    {
        MailReceiver[] expected = [CreateRandomMailReceiver()];
        mailReceiverBrokerMock.Setup(expression: broker => broker.GetEnabledMailReceivers())
            .Returns(value: expected);

        MailReceiver[] actual = mailReceiverService.GetEnabled();

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task DeleteAllMailReceiverAsyncShouldDelegateToBrokerAsync()
    {
        MailReceiver[] receivers = [CreateRandomMailReceiver()];
        mailReceiverBrokerMock.Setup(expression: broker =>
                broker.DeleteAllMailReceiversAsync(receivers))
            .Returns(value: ValueTask.CompletedTask);

        await mailReceiverService.DeleteAllMailReceiverAsync(deletedMailReceiver: receivers);

        mailReceiverBrokerMock.Verify(expression: broker =>
            broker.DeleteAllMailReceiversAsync(receivers), Times.Once);
    }

    [Fact]
    public async Task DeleteAllByAppIdAsyncShouldDelegateToBrokerAsync()
    {
        mailReceiverBrokerMock.Setup(expression: broker =>
                broker.DeleteAllMailReceiversByAppIdAsync(7))
            .Returns(value: ValueTask.CompletedTask);

        await mailReceiverService.DeleteAllByAppIdAsync(appId: 7);

        mailReceiverBrokerMock.Verify(expression: broker =>
            broker.DeleteAllMailReceiversByAppIdAsync(7), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005