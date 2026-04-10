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
    public async Task ShouldPassThroughCallWhenRaiseQueuedEmailDeleteEventAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailEventServiceMock
            .Setup(x => x.RaiseQueuedEmailDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseQueuedEmailDeleteEventAsync(entity);

        // Then
        queuedEmailEventServiceMock.Verify(x => x.RaiseQueuedEmailDeleteEventAsync(entity), Times.Once);
        queuedEmailEventServiceMock.VerifyNoOtherCalls();
    }

}








