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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        sentEmailProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        sentEmailEventProcessingServiceMock
            .Setup(x => x.RaiseSentEmailUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        SentEmail result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        sentEmailProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        sentEmailEventProcessingServiceMock.Verify(x => x.RaiseSentEmailUpdateEventAsync(entity), Times.Once);
    }

}








