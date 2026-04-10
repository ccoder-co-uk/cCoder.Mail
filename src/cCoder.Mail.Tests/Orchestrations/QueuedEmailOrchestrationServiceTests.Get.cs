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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        QueuedEmail result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        queuedEmailProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        queuedEmailProcessingServiceMock.VerifyNoOtherCalls();
        queuedEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








