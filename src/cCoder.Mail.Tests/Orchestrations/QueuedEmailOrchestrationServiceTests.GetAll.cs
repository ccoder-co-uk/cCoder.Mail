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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<QueuedEmail> entities = new[] { CreateRandomQueuedEmail() }.AsQueryable();

        queuedEmailProcessingServiceMock.Setup(expression: x => x.GetAllQueuedEmail(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<QueuedEmail> result = orchestrationService.GetAllQueuedEmail(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        queuedEmailProcessingServiceMock.Verify(expression: x => x.GetAllQueuedEmail(ignoreFilters: true), times: Times.Once);
        queuedEmailProcessingServiceMock.VerifyNoOtherCalls();
        queuedEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}