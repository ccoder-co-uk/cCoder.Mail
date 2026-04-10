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
    public async Task ShouldPassThroughCallWhenRaiseSentEmailDeleteEventAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        sentEmailEventServiceMock
            .Setup(x => x.RaiseSentEmailDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseSentEmailDeleteEventAsync(entity);

        // Then
        sentEmailEventServiceMock.Verify(x => x.RaiseSentEmailDeleteEventAsync(entity), Times.Once);
        sentEmailEventServiceMock.VerifyNoOtherCalls();
    }

}








