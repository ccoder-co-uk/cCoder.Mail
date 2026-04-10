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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        MailServer entity = CreateRandomMailServer();
        mailServerProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { entity }.AsQueryable());
        mailServerProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        mailServerEventProcessingServiceMock
            .Setup(x => x.RaiseMailServerDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        mailServerProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        mailServerProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        mailServerEventProcessingServiceMock.Verify(x => x.RaiseMailServerDeleteEventAsync(entity), Times.Once);
    }

}








