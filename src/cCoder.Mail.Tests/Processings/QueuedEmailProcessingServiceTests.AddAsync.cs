// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        queuedEmailServiceMock.Setup(expression: x => x.AddQueuedEmailAsync(newQueuedEmail: email, checkPrivileges: false))
            .ReturnsAsync(value: email);

        // When
        QueuedEmail result = await queuedEmailProcessingService.AddQueuedEmailAsync(newQueuedEmail: email, checkPrivs: false);

        // Then

        result.Should()
            .BeSameAs(expected: email);

        queuedEmailServiceMock.Verify(expression: x => x.AddQueuedEmailAsync(newQueuedEmail: email, checkPrivileges: false), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToFoundationUsingDefaultPathForAddAsync()
    {
        // Given
        QueuedEmail email = CreateRandomQueuedEmail();

        queuedEmailServiceMock.Setup(expression: x => x.AddQueuedEmailAsync(newQueuedEmail: email, checkPrivileges: false))
            .ReturnsAsync(value: email);

        // When
        QueuedEmail result = await queuedEmailProcessingService.AddQueuedEmailAsync(newQueuedEmail: email);

        // Then

        result.Should()
            .BeSameAs(expected: email);

        queuedEmailServiceMock.Verify(expression: x => x.AddQueuedEmailAsync(newQueuedEmail: email, checkPrivileges: false), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}