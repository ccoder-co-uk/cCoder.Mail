using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class SentEmailProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        var id = entity.Id;
        sentEmailServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await sentEmailProcessingService.DeleteAsync(id);

        // Then
        sentEmailServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        sentEmailServiceMock.VerifyNoOtherCalls();
    }

}








