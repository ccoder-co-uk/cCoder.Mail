// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Orchestrations;

public partial class MailReceiverOrchestrationServiceTests
{
    [Fact]
    public async Task ExistsAsyncShouldReturnWhetherReceiverExists()
    {
        Guid receiverId = Guid.NewGuid();
        mailReceiverProcessingServiceMock.Setup(service => service.GetAllMailReceiver(true))
            .Returns(new[] { new MailReceiver { Id = receiverId } }.AsQueryable());

        bool exists = await mailReceiverOrchestrationService.ExistsAsync(receiverId);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task AddMailReceiverAsyncShouldDelegateToProcessingAsync()
    {
        var receiver = new MailReceiver { Id = Guid.NewGuid() };
        mailReceiverProcessingServiceMock.Setup(service => service.AddMailReceiverAsync(receiver)).ReturnsAsync(receiver);

        MailReceiver result = await mailReceiverOrchestrationService.AddMailReceiverAsync(receiver);

        result.Should().BeSameAs(receiver);
    }

    [Fact]
    public async Task UpdateMailReceiverAsyncShouldDelegateToProcessingAsync()
    {
        var receiver = new MailReceiver { Id = Guid.NewGuid() };
        mailReceiverProcessingServiceMock.Setup(service => service.UpdateMailReceiverAsync(receiver)).ReturnsAsync(receiver);

        MailReceiver result = await mailReceiverOrchestrationService.UpdateMailReceiverAsync(receiver);

        result.Should().BeSameAs(receiver);
    }

    [Fact]
    public async Task DeleteByAppIdAsyncShouldDelegateToProcessingAsync()
    {
        mailReceiverProcessingServiceMock.Setup(service => service.DeleteByAppIdAsync(7)).Returns(ValueTask.CompletedTask);

        await mailReceiverOrchestrationService.DeleteByAppIdAsync(7);

        mailReceiverProcessingServiceMock.Verify(service => service.DeleteByAppIdAsync(7), Times.Once);
    }

    [Fact]
    public async Task RunAsyncShouldReceiveAndPersistOnlyNewEmailsAsync()
    {
        DateTimeOffset previousReceivedOn = DateTimeOffset.UtcNow.AddHours(-2);
        Guid receiverId = Guid.NewGuid();
        var receiver = new MailReceiver { Id = receiverId, AppId = 7, ProviderName = "POP3", LastReceivedOn = previousReceivedOn };
        var existingEmail = new ReceivedEmail { MessageId = "existing" };
        var newEmail = new ReceivedEmail { MessageId = "new" };
        mailReceiverProcessingServiceMock.Setup(service => service.GetEnabled()).Returns([receiver]);
        mailReceivingProcessingServiceMock.Setup(service => service.ReceiveMailboxReceiveRequestAsync(
                It.IsAny<global::cCoder.Mail.Providers.Models.MailboxReceiveRequest>(), CancellationToken.None))
            .ReturnsAsync([existingEmail, newEmail]);
        receivedEmailProcessingServiceMock.Setup(service => service.Exists(receiverId, "existing")).Returns(true);
        receivedEmailProcessingServiceMock.Setup(service => service.Exists(receiverId, "new")).Returns(false);
        receivedEmailProcessingServiceMock.Setup(service => service.AddRangeReceivedEmailAsync(
                It.Is<IEnumerable<ReceivedEmail>>(emails => emails.Single() == newEmail), CancellationToken.None))
            .Returns(ValueTask.CompletedTask);
        mailReceiverProcessingServiceMock.Setup(service => service.UpdateMailReceiverAsync(receiver)).ReturnsAsync(receiver);

        await mailReceiverOrchestrationService.RunAsync();

        newEmail.AppId.Should().Be(7);
        newEmail.MailReceiverId.Should().Be(receiverId);
        newEmail.SentByUserId.Should().Be("Guest");
        newEmail.ReceivedOn.Should().NotBe(default);
        receiver.LastReceivedOn.Should().BeAfter(previousReceivedOn);
    }

    [Fact]
    public async Task RunAsyncShouldPreserveExistingEmailValuesAsync()
    {
        Guid receiverId = Guid.NewGuid();
        DateTimeOffset receivedOn = DateTimeOffset.UtcNow.AddHours(-1);
        var receiver = new MailReceiver { Id = receiverId, AppId = 3 };
        var email = new ReceivedEmail { MessageId = "new", SentByUserId = "user", ReceivedOn = receivedOn };
        mailReceiverProcessingServiceMock.Setup(service => service.GetEnabled()).Returns([receiver]);
        mailReceivingProcessingServiceMock.Setup(service => service.ReceiveMailboxReceiveRequestAsync(
                It.IsAny<global::cCoder.Mail.Providers.Models.MailboxReceiveRequest>(), CancellationToken.None)).ReturnsAsync([email]);
        receivedEmailProcessingServiceMock.Setup(service => service.AddRangeReceivedEmailAsync(
                It.IsAny<IEnumerable<ReceivedEmail>>(), CancellationToken.None)).Returns(ValueTask.CompletedTask);
        mailReceiverProcessingServiceMock.Setup(service => service.UpdateMailReceiverAsync(receiver)).ReturnsAsync(receiver);

        await mailReceiverOrchestrationService.RunAsync();

        email.SentByUserId.Should().Be("user");
        email.ReceivedOn.Should().Be(receivedOn);
    }

    [Fact]
    public async Task RunContinuouslyAsyncShouldReturnDuringMigrationAsync()
    {
        mailReceivingProcessingServiceMock.Setup(service => service.IsMigrationInProgress()).Returns(true);

        await mailReceiverOrchestrationService.RunContinuouslyAsync();

        mailReceiverProcessingServiceMock.Verify(service => service.GetEnabled(), Times.Never);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005