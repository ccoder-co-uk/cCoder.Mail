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


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class QueuedEmailOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();

        queuedEmailProcessingServiceMock.Setup(expression: x => x.UpdateQueuedEmailAsync(updatedQueuedEmail: entity))
            .ReturnsAsync(value: entity);

        queuedEmailEventProcessingServiceMock
            .Setup(expression: x => x.RaiseQueuedEmailUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        QueuedEmail result = await orchestrationService.UpdateQueuedEmailAsync(updatedQueuedEmail: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        queuedEmailProcessingServiceMock.Verify(expression: x => x.UpdateQueuedEmailAsync(updatedQueuedEmail: entity), times: Times.Once);
        queuedEmailEventProcessingServiceMock.Verify(expression: x => x.RaiseQueuedEmailUpdateEventAsync(entity: entity), times: Times.Once);
    }

}