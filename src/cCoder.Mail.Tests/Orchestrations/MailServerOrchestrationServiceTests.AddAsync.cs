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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        mailServerProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        mailServerEventProcessingServiceMock
            .Setup(x => x.RaiseMailServerAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        MailServer result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        mailServerProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        mailServerEventProcessingServiceMock.Verify(x => x.RaiseMailServerAddEventAsync(entity), Times.Once);
    }

}








