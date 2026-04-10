using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class SentEmailOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        SentEmail entity = CreateRandomSentEmail();
        sentEmailProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { entity }.AsQueryable());
        sentEmailProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        sentEmailEventProcessingServiceMock
            .Setup(x => x.RaiseSentEmailDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        sentEmailProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        sentEmailProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        sentEmailEventProcessingServiceMock.Verify(x => x.RaiseSentEmailDeleteEventAsync(entity), Times.Once);
    }

}








