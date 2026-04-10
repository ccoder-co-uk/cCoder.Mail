using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class SentEmailEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseSentEmailAddEventAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        sentEmailEventServiceMock
            .Setup(x => x.RaiseSentEmailAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseSentEmailAddEventAsync(entity);

        // Then
        sentEmailEventServiceMock.Verify(x => x.RaiseSentEmailAddEventAsync(entity), Times.Once);
        sentEmailEventServiceMock.VerifyNoOtherCalls();
    }

}








