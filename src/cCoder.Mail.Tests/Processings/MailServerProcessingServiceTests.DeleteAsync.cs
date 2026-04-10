using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        var id = entity.Id;
        mailServerServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await mailServerProcessingService.DeleteAsync(id);

        // Then
        mailServerServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}








