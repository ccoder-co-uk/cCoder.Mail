using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class QueuedEmailOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        queuedEmailEventProcessingServiceMock
            .Setup(x => x.RaiseQueuedEmailAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        QueuedEmail result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        queuedEmailProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        queuedEmailEventProcessingServiceMock.Verify(x => x.RaiseQueuedEmailAddEventAsync(entity), Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseAddEventAsyncWhenAddAsync()
    {
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailProcessingServiceMock.Setup(x => x.AddAsync(entity, false)).ReturnsAsync(entity);
        queuedEmailEventProcessingServiceMock
            .Setup(x => x.RaiseQueuedEmailAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        QueuedEmail result = await orchestrationService.AddAsync(entity, false);

        result.Should().BeSameAs(entity);
        queuedEmailProcessingServiceMock.Verify(x => x.AddAsync(entity, false), Times.Once);
        queuedEmailProcessingServiceMock.VerifyNoOtherCalls();
        queuedEmailEventProcessingServiceMock.Verify(x => x.RaiseQueuedEmailAddEventAsync(entity), Times.Once);
        queuedEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








