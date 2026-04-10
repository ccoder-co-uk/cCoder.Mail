using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<QueuedEmail> entities = new[] { CreateRandomQueuedEmail() }.AsQueryable();
        queuedEmailServiceMock.Setup(x => x.GetAll()).Returns(entities);

        // When
        IQueryable<QueuedEmail> result = queuedEmailProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(entities);
        queuedEmailServiceMock.Verify(x => x.GetAll(), Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
    }

}








