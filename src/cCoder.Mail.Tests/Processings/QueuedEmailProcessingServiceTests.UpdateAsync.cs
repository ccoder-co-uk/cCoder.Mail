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
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();
        queuedEmailServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        // When
        QueuedEmail result = await queuedEmailProcessingService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        queuedEmailServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
    }

}








