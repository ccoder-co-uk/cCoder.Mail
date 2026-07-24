// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseQueuedEmailAddEventAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();

        queuedEmailEventServiceMock
            .Setup(expression: x => x.RaiseQueuedEmailAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseQueuedEmailAddEventAsync(entity: entity);

        // Then
        queuedEmailEventServiceMock.Verify(expression: x => x.RaiseQueuedEmailAddEventAsync(entity: entity), times: Times.Once);
        queuedEmailEventServiceMock.VerifyNoOtherCalls();
    }

}