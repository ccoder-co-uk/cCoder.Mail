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
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();

        queuedEmailServiceMock.Setup(expression: x => x.UpdateQueuedEmailAsync(updatedQueuedEmail: entity))
            .ReturnsAsync(value: entity);

        // When
        QueuedEmail result = await queuedEmailProcessingService.UpdateQueuedEmailAsync(updatedQueuedEmail: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        queuedEmailServiceMock.Verify(expression: x => x.UpdateQueuedEmailAsync(updatedQueuedEmail: entity), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
    }

}