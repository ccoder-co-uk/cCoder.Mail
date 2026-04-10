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
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();
        var id = entity.Id;
        queuedEmailServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        QueuedEmail result = queuedEmailProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        queuedEmailServiceMock.Verify(x => x.Get(id), Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
    }

}








