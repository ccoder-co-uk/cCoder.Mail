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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<QueuedEmail> entities = new[] { CreateRandomQueuedEmail() }.AsQueryable();
        queuedEmailProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<QueuedEmail> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        queuedEmailProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        queuedEmailProcessingServiceMock.VerifyNoOtherCalls();
        queuedEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








