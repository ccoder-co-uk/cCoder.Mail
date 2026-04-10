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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        queuedEmailEventProcessingServiceMock
            .Setup(x => x.RaiseQueuedEmailUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        QueuedEmail result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        queuedEmailProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        queuedEmailEventProcessingServiceMock.Verify(x => x.RaiseQueuedEmailUpdateEventAsync(entity), Times.Once);
    }

}








