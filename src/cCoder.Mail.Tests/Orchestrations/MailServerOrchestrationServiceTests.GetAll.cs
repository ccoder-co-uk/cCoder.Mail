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

public partial class MailServerOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<MailServer> entities = new[] { CreateRandomMailServer() }.AsQueryable();

        mailServerProcessingServiceMock.Setup(expression: x => x.GetAllMailServer(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<MailServer> result = orchestrationService.GetAllMailServer(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        mailServerProcessingServiceMock.Verify(expression: x => x.GetAllMailServer(ignoreFilters: true), times: Times.Once);
        mailServerProcessingServiceMock.VerifyNoOtherCalls();
        mailServerEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}