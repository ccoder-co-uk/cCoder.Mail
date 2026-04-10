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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<MailServer> entities = new[] { CreateRandomMailServer() }.AsQueryable();
        mailServerProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<MailServer> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        mailServerProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        mailServerProcessingServiceMock.VerifyNoOtherCalls();
        mailServerEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








