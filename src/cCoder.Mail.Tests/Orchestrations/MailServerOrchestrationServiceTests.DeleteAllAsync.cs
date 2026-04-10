using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class MailServerOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        MailServer[] entities = [CreateRandomMailServer()];
        mailServerProcessingServiceMock.Setup(x => x.DeleteAllAsync(entities)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        mailServerProcessingServiceMock.Verify(x => x.DeleteAllAsync(entities), Times.Once);
        mailServerProcessingServiceMock.VerifyNoOtherCalls();
        mailServerEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








