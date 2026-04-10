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
    public async Task ShouldDelegateToFoundationWithRequestedPrivilegeModeForAddAsync()
    {
        // Given
        QueuedEmail email = CreateRandomQueuedEmail();
        queuedEmailServiceMock.Setup(x => x.AddAsync(email, false)).ReturnsAsync(email);

        // When
        QueuedEmail result = await queuedEmailProcessingService.AddAsync(email, checkPrivs: false);

        // Then
        result.Should().BeSameAs(email);
        queuedEmailServiceMock.Verify(x => x.AddAsync(email, false), Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToFoundationUsingDefaultPathForAddAsync()
    {
        // Given
        QueuedEmail email = CreateRandomQueuedEmail();
        queuedEmailServiceMock.Setup(x => x.AddAsync(email, false)).ReturnsAsync(email);

        // When
        QueuedEmail result = await queuedEmailProcessingService.AddAsync(email);

        // Then
        result.Should().BeSameAs(email);
        queuedEmailServiceMock.Verify(x => x.AddAsync(email, false), Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}

