using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class SentEmailOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        SentEmail entity = CreateRandomSentEmail();
        sentEmailProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        SentEmail result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        sentEmailProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        sentEmailProcessingServiceMock.VerifyNoOtherCalls();
        sentEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








