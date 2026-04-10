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
    public async Task ShouldPassThroughCallWhenRaiseMailServerAddEventAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        mailServerEventServiceMock
            .Setup(x => x.RaiseMailServerAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseMailServerAddEventAsync(entity);

        // Then
        mailServerEventServiceMock.Verify(x => x.RaiseMailServerAddEventAsync(entity), Times.Once);
        mailServerEventServiceMock.VerifyNoOtherCalls();
    }

}








