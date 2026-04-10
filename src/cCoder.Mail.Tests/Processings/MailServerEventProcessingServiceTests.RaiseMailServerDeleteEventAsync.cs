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
    public async Task ShouldPassThroughCallWhenRaiseMailServerDeleteEventAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        mailServerEventServiceMock
            .Setup(x => x.RaiseMailServerDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseMailServerDeleteEventAsync(entity);

        // Then
        mailServerEventServiceMock.Verify(x => x.RaiseMailServerDeleteEventAsync(entity), Times.Once);
        mailServerEventServiceMock.VerifyNoOtherCalls();
    }

}








