using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        mailServerServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        // When
        MailServer result = await mailServerProcessingService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        mailServerServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}








