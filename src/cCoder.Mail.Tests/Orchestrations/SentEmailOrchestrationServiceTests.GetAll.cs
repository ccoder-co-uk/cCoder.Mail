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

public partial class SentEmailOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<SentEmail> entities = new[] { CreateRandomSentEmail() }.AsQueryable();

        sentEmailProcessingServiceMock.Setup(expression: x => x.GetAllSentEmail(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<SentEmail> result = orchestrationService.GetAllSentEmail(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        sentEmailProcessingServiceMock.Verify(expression: x => x.GetAllSentEmail(ignoreFilters: true), times: Times.Once);
        sentEmailProcessingServiceMock.VerifyNoOtherCalls();
        sentEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}