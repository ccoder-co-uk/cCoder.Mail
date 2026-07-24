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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();

        queuedEmailProcessingServiceMock.Setup(expression: x => x.AddQueuedEmailAsync(newQueuedEmail: entity))
            .ReturnsAsync(value: entity);

        queuedEmailEventProcessingServiceMock
            .Setup(expression: x => x.RaiseQueuedEmailAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        QueuedEmail result = await orchestrationService.AddQueuedEmailAsync(newQueuedEmail: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        queuedEmailProcessingServiceMock.Verify(expression: x => x.AddQueuedEmailAsync(newQueuedEmail: entity), times: Times.Once);
        queuedEmailEventProcessingServiceMock.Verify(expression: x => x.RaiseQueuedEmailAddEventAsync(entity: entity), times: Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseAddEventAsyncWhenAddAsync()
    {
        QueuedEmail entity = CreateRandomQueuedEmail();

        queuedEmailProcessingServiceMock.Setup(expression: x => x.AddQueuedEmailAsync(newQueuedEmail: entity, checkPrivs: false))
            .ReturnsAsync(value: entity);

        queuedEmailEventProcessingServiceMock
            .Setup(expression: x => x.RaiseQueuedEmailAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        QueuedEmail result = await orchestrationService.AddQueuedEmailAsync(newQueuedEmail: entity, checkPrivs: false);

        result.Should()
            .BeSameAs(expected: entity);

        queuedEmailProcessingServiceMock.Verify(expression: x => x.AddQueuedEmailAsync(newQueuedEmail: entity, checkPrivs: false), times: Times.Once);
        queuedEmailProcessingServiceMock.VerifyNoOtherCalls();
        queuedEmailEventProcessingServiceMock.Verify(expression: x => x.RaiseQueuedEmailAddEventAsync(entity: entity), times: Times.Once);
        queuedEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}