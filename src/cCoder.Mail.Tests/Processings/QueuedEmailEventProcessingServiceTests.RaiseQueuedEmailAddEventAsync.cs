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
    public async Task ShouldPassThroughCallWhenRaiseQueuedEmailAddEventAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailEventServiceMock
            .Setup(x => x.RaiseQueuedEmailAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseQueuedEmailAddEventAsync(entity);

        // Then
        queuedEmailEventServiceMock.Verify(x => x.RaiseQueuedEmailAddEventAsync(entity), Times.Once);
        queuedEmailEventServiceMock.VerifyNoOtherCalls();
    }

}








