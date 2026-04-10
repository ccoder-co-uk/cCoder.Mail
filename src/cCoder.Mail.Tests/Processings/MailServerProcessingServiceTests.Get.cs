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
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        MailServer entity = CreateRandomMailServer();
        var id = entity.Id;
        mailServerServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        MailServer result = mailServerProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        mailServerServiceMock.Verify(x => x.Get(id), Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}








