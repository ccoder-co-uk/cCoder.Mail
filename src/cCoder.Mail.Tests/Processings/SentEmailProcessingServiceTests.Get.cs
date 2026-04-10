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
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();
        var id = entity.Id;
        sentEmailServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        SentEmail result = sentEmailProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        sentEmailServiceMock.Verify(x => x.Get(id), Times.Once);
        sentEmailServiceMock.VerifyNoOtherCalls();
    }

}








