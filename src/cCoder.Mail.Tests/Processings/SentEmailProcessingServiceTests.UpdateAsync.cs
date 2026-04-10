using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class SentEmailProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        sentEmailServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        // When
        SentEmail result = await sentEmailProcessingService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        sentEmailServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        sentEmailServiceMock.VerifyNoOtherCalls();
    }

}








