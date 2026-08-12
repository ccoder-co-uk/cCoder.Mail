// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class QueuedEmailServiceTests
{
    [Fact]
    public void GetDispatchBatchShouldDelegate()
    {
        QueuedEmail[] emails = [CreateRandomQueuedEmail()];
        queuedEmailBrokerMock.Setup(x => x.GetDispatchBatch(5, 3)).Returns(emails);
        queuedEmailService.GetDispatchBatch(5, 3).Should().BeSameAs(emails);
    }

    [Fact]
    public async Task RecordSendFailureAsyncShouldDelegateAsync()
    {
        queuedEmailBrokerMock.Setup(x => x.AddQueuedEmailSendFailureAsync(1, "reason", CancellationToken.None))
            .Returns(ValueTask.CompletedTask);
        await queuedEmailService.RecordSendFailureAsync(1, "reason");
        queuedEmailBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task MarkAsSentQueuedEmailAsyncShouldCopyAndDelegateAsync()
    {
        QueuedEmail email = CreateRandomQueuedEmail(); Guid senderId = Guid.NewGuid();
        queuedEmailBrokerMock.Setup(x => x.MarkQueuedEmailAsSentAsync(
                It.Is<QueuedEmail>(copy => copy != email && copy.Id == email.Id),
                senderId, "from@example.test", CancellationToken.None)).Returns(ValueTask.CompletedTask);
        await queuedEmailService.MarkAsSentQueuedEmailAsync(email, senderId, "from@example.test");
        queuedEmailBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task DeleteAllForAppQueuedEmailAsyncShouldDeleteEachWithoutAuthorizationAsync()
    {
        QueuedEmail email = CreateRandomQueuedEmail();
        email.FailedSends = [new EmailSendFailure { Id = 2, EmailId = email.Id }, null];
        queuedEmailBrokerMock.Setup(x => x.GetAllQueuedEmailsIgnoringFilters()).Returns(new[] { email }.AsQueryable());
        queuedEmailBrokerMock.Setup(x => x.DeleteAllQueuedEmailSendFailuresAsync(
                It.Is<IEnumerable<EmailSendFailure>>(items => items.Count() == 2))).Returns(ValueTask.CompletedTask);
        queuedEmailBrokerMock.Setup(x => x.DeleteQueuedEmailAsync(It.Is<QueuedEmail>(copy => copy.Id == email.Id))).ReturnsAsync(1);
        await queuedEmailService.DeleteAllForAppQueuedEmailAsync([email]);
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Never);
    }

    [Fact]
    public async Task DeleteAllForAppQueuedEmailAsyncShouldRejectNullCollectionAsync()
    {
        Func<Task> action = async () => await queuedEmailService.DeleteAllForAppQueuedEmailAsync(null);

        await action.Should().ThrowAsync<global::cCoder.Mail.Providers.Models.Exceptions.MailValidationException>();
        queuedEmailBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsyncShouldReturnWhenEmailDoesNotExistAsync()
    {
        queuedEmailBrokerMock.Setup(x => x.GetAllQueuedEmailsIgnoringFilters()).Returns(Array.Empty<QueuedEmail>().AsQueryable());
        await queuedEmailService.DeleteAsync(99, false);
        queuedEmailBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task DeleteAllByAppIdAsyncShouldDelegateAsync()
    {
        queuedEmailBrokerMock.Setup(x => x.DeleteAllQueuedEmailsByAppIdAsync(7)).Returns(ValueTask.CompletedTask);
        await queuedEmailService.DeleteAllByAppIdAsync(7);
        queuedEmailBrokerMock.VerifyAll();
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005