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
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<MailServer> entities = new[] { CreateRandomMailServer() }.AsQueryable();
        mailServerServiceMock.Setup(x => x.GetAll()).Returns(entities);

        // When
        IQueryable<MailServer> result = mailServerProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(entities);
        mailServerServiceMock.Verify(x => x.GetAll(), Times.Once);
        mailServerServiceMock.VerifyNoOtherCalls();
    }

}








