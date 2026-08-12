// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class ReceivedEmailServiceTests
{
    [Fact]
    public async Task AddRangeReceivedEmailAsyncShouldDelegateToBrokerAsync()
    {
        ReceivedEmail[] receivedEmails = [CreateRandomReceivedEmail()];
        receivedEmailBrokerMock.Setup(expression: broker => broker.AddReceivedEmailsAsync(
                It.IsAny<IEnumerable<ReceivedEmail>>(),
                CancellationToken.None))
            .Returns(value: ValueTask.CompletedTask);

        await receivedEmailService.AddRangeReceivedEmailAsync(receivedEmails);

        receivedEmailBrokerMock.Verify(expression: broker => broker.AddReceivedEmailsAsync(
            It.IsAny<IEnumerable<ReceivedEmail>>(),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ExistsShouldReturnBrokerResult()
    {
        Guid mailReceiverId = Guid.NewGuid();
        receivedEmailBrokerMock.Setup(expression: broker =>
                broker.Exists(mailReceiverId, "message"))
            .Returns(value: true);

        bool result = receivedEmailService.Exists(mailReceiverId, "message");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAllReceivedEmailAsyncShouldDelegateToBrokerAsync()
    {
        ReceivedEmail[] emails = [CreateRandomReceivedEmail()];
        receivedEmailBrokerMock.Setup(expression: broker =>
                broker.DeleteAllReceivedEmailsAsync(emails))
            .Returns(value: ValueTask.CompletedTask);

        await receivedEmailService.DeleteAllReceivedEmailAsync(emails);

        receivedEmailBrokerMock.Verify(expression: broker =>
            broker.DeleteAllReceivedEmailsAsync(emails), Times.Once);
    }

    [Fact]
    public async Task DeleteAllByAppIdAsyncShouldDelegateToBrokerAsync()
    {
        receivedEmailBrokerMock.Setup(expression: broker =>
                broker.DeleteAllReceivedEmailsByAppIdAsync(7))
            .Returns(value: ValueTask.CompletedTask);

        await receivedEmailService.DeleteAllByAppIdAsync(appId: 7);

        receivedEmailBrokerMock.Verify(expression: broker =>
            broker.DeleteAllReceivedEmailsByAppIdAsync(7), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005