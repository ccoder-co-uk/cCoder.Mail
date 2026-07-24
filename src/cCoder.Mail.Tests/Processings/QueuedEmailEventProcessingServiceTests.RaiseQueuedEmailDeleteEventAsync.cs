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
    public async Task ShouldPassThroughCallWhenRaiseQueuedEmailDeleteEventAsync()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();

        queuedEmailEventServiceMock
            .Setup(expression: x => x.RaiseQueuedEmailDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseQueuedEmailDeleteEventAsync(entity: entity);

        // Then
        queuedEmailEventServiceMock.Verify(expression: x => x.RaiseQueuedEmailDeleteEventAsync(entity: entity), times: Times.Once);
        queuedEmailEventServiceMock.VerifyNoOtherCalls();
    }

}