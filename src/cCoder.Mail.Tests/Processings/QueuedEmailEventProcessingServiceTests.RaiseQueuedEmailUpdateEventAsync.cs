using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseQueuedEmailUpdateEventAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailEventServiceMock
            .Setup(x => x.RaiseQueuedEmailUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseQueuedEmailUpdateEventAsync(entity);

        // Then
        queuedEmailEventServiceMock.Verify(x => x.RaiseQueuedEmailUpdateEventAsync(entity), Times.Once);
        queuedEmailEventServiceMock.VerifyNoOtherCalls();
    }

}








