// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public partial class SentEmailOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        SentEmail entity = CreateRandomSentEmail();

        sentEmailProcessingServiceMock.Setup(expression: x => x.GetAllSentEmail(ignoreFilters: true))
            .Returns(value: new[] { entity }.AsQueryable());

        sentEmailProcessingServiceMock.Setup(expression: x => x.DeleteAsync(iSentEmailId: id))
            .Returns(value: ValueTask.CompletedTask);

        sentEmailEventProcessingServiceMock
            .Setup(expression: x => x.RaiseSentEmailDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(sentEmailId: id);

        // Then
        sentEmailProcessingServiceMock.Verify(expression: x => x.GetAllSentEmail(ignoreFilters: true), times: Times.Once);
        sentEmailProcessingServiceMock.Verify(expression: x => x.DeleteAsync(iSentEmailId: id), times: Times.Once);
        sentEmailEventProcessingServiceMock.Verify(expression: x => x.RaiseSentEmailDeleteEventAsync(entity: entity), times: Times.Once);
    }

}