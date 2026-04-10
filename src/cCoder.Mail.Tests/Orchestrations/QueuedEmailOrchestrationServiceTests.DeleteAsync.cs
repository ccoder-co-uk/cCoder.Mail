using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class QueuedEmailOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { entity }.AsQueryable());
        queuedEmailProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        queuedEmailEventProcessingServiceMock
            .Setup(x => x.RaiseQueuedEmailDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        queuedEmailProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        queuedEmailProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        queuedEmailEventProcessingServiceMock.Verify(x => x.RaiseQueuedEmailDeleteEventAsync(entity), Times.Once);
    }

}








