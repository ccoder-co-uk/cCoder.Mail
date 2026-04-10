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
    public async Task ShouldPassThroughCallWhenRaiseSentEmailUpdateEventAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        sentEmailEventServiceMock
            .Setup(x => x.RaiseSentEmailUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseSentEmailUpdateEventAsync(entity);

        // Then
        sentEmailEventServiceMock.Verify(x => x.RaiseSentEmailUpdateEventAsync(entity), Times.Once);
        sentEmailEventServiceMock.VerifyNoOtherCalls();
    }

}








