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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        sentEmailProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        sentEmailEventProcessingServiceMock
            .Setup(x => x.RaiseSentEmailAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        SentEmail result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        sentEmailProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        sentEmailEventProcessingServiceMock.Verify(x => x.RaiseSentEmailAddEventAsync(entity), Times.Once);
    }

}








