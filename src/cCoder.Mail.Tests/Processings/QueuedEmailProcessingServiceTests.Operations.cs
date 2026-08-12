// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{
    [Fact]
    public void GetDispatchBatchShouldDelegateToFoundation()
    {
        QueuedEmail[] emails = [CreateRandomQueuedEmail()];
        queuedEmailServiceMock.Setup(service => service.GetDispatchBatch(5, 3)).Returns(emails);

        QueuedEmail[] result = queuedEmailProcessingService.GetDispatchBatch(5, 3);

        result.Should().BeSameAs(emails);
    }

    [Fact]
    public async Task RecordSendFailureAsyncShouldDelegateToFoundationAsync()
    {
        queuedEmailServiceMock.Setup(service => service.RecordSendFailureAsync(1, "reason", CancellationToken.None))
            .Returns(ValueTask.CompletedTask);

        await queuedEmailProcessingService.RecordSendFailureAsync(1, "reason");

        queuedEmailServiceMock.Verify(service => service.RecordSendFailureAsync(1, "reason", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task MarkAsSentQueuedEmailAsyncShouldDelegateToFoundationAsync()
    {
        QueuedEmail email = CreateRandomQueuedEmail();
        Guid senderId = Guid.NewGuid();
        queuedEmailServiceMock.Setup(service => service.MarkAsSentQueuedEmailAsync(
                email, senderId, "from@example.test", CancellationToken.None)).Returns(ValueTask.CompletedTask);

        await queuedEmailProcessingService.MarkAsSentQueuedEmailAsync(email, senderId, "from@example.test");

        queuedEmailServiceMock.VerifyAll();
    }

    [Fact]
    public async Task RetryAsyncShouldReturnWhenEmailDoesNotExistAsync()
    {
        queuedEmailServiceMock.Setup(service => service.GetQueuedEmail(17)).Returns((QueuedEmail)null);

        await queuedEmailProcessingService.RetryAsync(17);

        authorizationBrokerMock.Verify(service => service.GetCurrentUser(), Times.Never);
    }

    [Fact]
    public async Task DeleteByAppIdAsyncShouldDelegateToFoundationAsync()
    {
        queuedEmailServiceMock.Setup(service => service.DeleteAllByAppIdAsync(7)).Returns(ValueTask.CompletedTask);

        await queuedEmailProcessingService.DeleteByAppIdAsync(7);

        queuedEmailServiceMock.VerifyAll();
    }

    [Fact]
    public async Task AddOrUpdateQueuedEmailResultShouldAddNewEmailAsync()
    {
        QueuedEmail email = CreateRandomQueuedEmail();
        email.Id = 0;
        queuedEmailServiceMock.Setup(service => service.AddQueuedEmailAsync(email, true)).ReturnsAsync(email);

        IEnumerable<global::cCoder.Mail.Models.Result<QueuedEmail>> results =
            await queuedEmailProcessingService.AddOrUpdateQueuedEmailResult([email]);

        results.Single().Success.Should().BeTrue();
        results.Single().Message.Should().Be("Added Successfully");
    }

    [Fact]
    public async Task AddOrUpdateQueuedEmailResultShouldUpdateExistingEmailAsync()
    {
        QueuedEmail email = CreateRandomQueuedEmail();
        queuedEmailServiceMock.Setup(service => service.GetAllQueuedEmail(true)).Returns(new[] { email }.AsQueryable());
        queuedEmailServiceMock.Setup(service => service.UpdateQueuedEmailAsync(email)).ReturnsAsync(email);

        IEnumerable<global::cCoder.Mail.Models.Result<QueuedEmail>> results =
            await queuedEmailProcessingService.AddOrUpdateQueuedEmailResult([email]);

        results.Single().Success.Should().BeTrue();
        results.Single().Message.Should().Be("Updated Successfully");
    }

    [Fact]
    public async Task AddOrUpdateQueuedEmailResultShouldReportIndividualFailureAsync()
    {
        QueuedEmail email = CreateRandomQueuedEmail();
        email.Id = 0;
        queuedEmailServiceMock.Setup(service => service.AddQueuedEmailAsync(email, true))
            .ThrowsAsync(new InvalidOperationException("failed"));

        IEnumerable<global::cCoder.Mail.Models.Result<QueuedEmail>> results =
            await queuedEmailProcessingService.AddOrUpdateQueuedEmailResult([email]);

        results.Single().Success.Should().BeFalse();
        results.Single().Item.Should().BeSameAs(email);
        results.Single().Message.Should().Be("failed");
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005