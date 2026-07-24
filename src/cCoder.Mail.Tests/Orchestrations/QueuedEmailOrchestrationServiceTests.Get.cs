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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        QueuedEmail entity = CreateRandomQueuedEmail();

        queuedEmailProcessingServiceMock.Setup(expression: x => x.Get(id: id))
            .Returns(value: entity);

        // When
        QueuedEmail result = orchestrationService.Get(id: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        queuedEmailProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        queuedEmailProcessingServiceMock.VerifyNoOtherCalls();
        queuedEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}