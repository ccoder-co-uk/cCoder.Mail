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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        mailServerProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        mailServerEventProcessingServiceMock
            .Setup(x => x.RaiseMailServerUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        MailServer result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        mailServerProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        mailServerEventProcessingServiceMock.Verify(x => x.RaiseMailServerUpdateEventAsync(entity), Times.Once);
    }

}








