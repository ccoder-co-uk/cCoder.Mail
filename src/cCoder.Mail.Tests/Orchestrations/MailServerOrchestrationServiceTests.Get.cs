using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class MailServerOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        MailServer entity = CreateRandomMailServer();
        mailServerProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        MailServer result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        mailServerProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        mailServerProcessingServiceMock.VerifyNoOtherCalls();
        mailServerEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








