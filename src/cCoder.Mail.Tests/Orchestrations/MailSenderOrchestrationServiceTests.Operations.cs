// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Orchestrations;

public partial class MailSenderOrchestrationServiceTests
{
    [Fact]
    public async Task ExistsAsyncShouldReturnWhetherSenderExists()
    {
        Guid senderId = Guid.NewGuid();
        mailSenderProcessingServiceMock.Setup(expression: service =>
                service.GetAllMailSender(true))
            .Returns(value: new[] { new MailSender { Id = senderId } }.AsQueryable());

        bool exists = await mailSenderOrchestrationService.ExistsAsync(senderId);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task AddMailSenderAsyncShouldDelegateToProcessingAsync()
    {
        var sender = new MailSender { Id = Guid.NewGuid() };
        mailSenderProcessingServiceMock.Setup(expression: service =>
                service.AddMailSenderAsync(sender))
            .ReturnsAsync(value: sender);

        MailSender result = await mailSenderOrchestrationService.AddMailSenderAsync(sender);

        result.Should().BeSameAs(sender);
    }

    [Fact]
    public async Task UpdateMailSenderAsyncShouldDelegateToProcessingAsync()
    {
        var sender = new MailSender { Id = Guid.NewGuid() };
        mailSenderProcessingServiceMock.Setup(expression: service =>
                service.UpdateMailSenderAsync(sender))
            .ReturnsAsync(value: sender);

        MailSender result = await mailSenderOrchestrationService.UpdateMailSenderAsync(sender);

        result.Should().BeSameAs(sender);
    }

    [Fact]
    public async Task DeleteByAppIdAsyncShouldDelegateToProcessingAsync()
    {
        mailSenderProcessingServiceMock.Setup(expression: service => service.DeleteByAppIdAsync(7))
            .Returns(value: ValueTask.CompletedTask);

        await mailSenderOrchestrationService.DeleteByAppIdAsync(7);

        mailSenderProcessingServiceMock.Verify(expression: service => service.DeleteByAppIdAsync(7), Times.Once);
    }

    [Fact]
    public async Task RunAsyncShouldCompleteWhenQueueIsEmptyAsync()
    {
        queuedEmailProcessingServiceMock.Setup(expression: service => service.GetDispatchBatch(10, 10))
            .Returns(value: []);

        await mailSenderOrchestrationService.RunAsync();

        mailSendingProcessingServiceMock.Verify(expression: service =>
            service.LogDispatch(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RunAsyncShouldRecordFailureWhenSenderIsMissingAsync()
    {
        var email = new QueuedEmail { Id = 1 };
        queuedEmailProcessingServiceMock.Setup(expression: service => service.GetDispatchBatch(10, 10))
            .Returns(value: [email]);
        queuedEmailProcessingServiceMock.Setup(expression: service => service.RecordSendFailureAsync(
                1,
                It.IsAny<string>(),
                CancellationToken.None))
            .Returns(value: ValueTask.CompletedTask);

        await mailSenderOrchestrationService.RunAsync();

        mailSendingProcessingServiceMock.Verify(expression: service => service.LogSummary(1, 0, 1), Times.Once);
    }

    [Fact]
    public async Task RunAsyncShouldMarkSuccessfullySentEmailAsync()
    {
        var sender = new MailSender { Id = Guid.NewGuid(), User = "sender@example.test" };
        var email = new QueuedEmail { Id = 1, MailSender = sender };
        queuedEmailProcessingServiceMock.Setup(expression: service => service.GetDispatchBatch(10, 10))
            .Returns(value: [email]);
        mailSendingProcessingServiceMock.Setup(expression: service =>
                service.SendQueuedEmailAsync(email, CancellationToken.None))
            .Returns(value: Task.CompletedTask);
        queuedEmailProcessingServiceMock.Setup(expression: service => service.MarkAsSentQueuedEmailAsync(
                email,
                sender.Id,
                sender.User,
                CancellationToken.None))
            .Returns(value: ValueTask.CompletedTask);

        await mailSenderOrchestrationService.RunAsync();

        mailSendingProcessingServiceMock.Verify(expression: service => service.LogSummary(1, 1, 0), Times.Once);
    }

    [Fact]
    public async Task RunAsyncShouldRecordNestedSendFailureAsync()
    {
        var sender = new MailSender { Id = Guid.NewGuid() };
        var email = new QueuedEmail { Id = 1, MailSender = sender };
        queuedEmailProcessingServiceMock.Setup(expression: service => service.GetDispatchBatch(10, 10))
            .Returns(value: [email]);
        mailSendingProcessingServiceMock.Setup(expression: service =>
                service.SendQueuedEmailAsync(email, CancellationToken.None))
            .ThrowsAsync(exception: new Exception("outer", new Exception("inner")));
        queuedEmailProcessingServiceMock.Setup(expression: service => service.RecordSendFailureAsync(
                1,
                It.Is<string>(reason => reason.Contains("outer") && reason.Contains("inner")),
                CancellationToken.None))
            .Returns(value: ValueTask.CompletedTask);

        await mailSenderOrchestrationService.RunAsync();

        mailSendingProcessingServiceMock.Verify(expression: service => service.LogSummary(1, 0, 1), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005