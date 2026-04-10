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
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<SentEmail> entities = new[] { CreateRandomSentEmail() }.AsQueryable();
        sentEmailServiceMock.Setup(x => x.GetAll()).Returns(entities);

        // When
        IQueryable<SentEmail> result = sentEmailProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(entities);
        sentEmailServiceMock.Verify(x => x.GetAll(), Times.Once);
        sentEmailServiceMock.VerifyNoOtherCalls();
    }

}








