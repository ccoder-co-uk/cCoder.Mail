using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseMailServerUpdateEventAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        mailServerEventServiceMock
            .Setup(x => x.RaiseMailServerUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseMailServerUpdateEventAsync(entity);

        // Then
        mailServerEventServiceMock.Verify(x => x.RaiseMailServerUpdateEventAsync(entity), Times.Once);
        mailServerEventServiceMock.VerifyNoOtherCalls();
    }

}








