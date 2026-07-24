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
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        SentEmail[] entities = [CreateRandomSentEmail()];

        sentEmailProcessingServiceMock.Setup(expression: x => x.DeleteAllSentEmailAsync(deletedSentEmail: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllSentEmailAsync(deletedSentEmail: entities);

        // Then
        sentEmailProcessingServiceMock.Verify(expression: x => x.DeleteAllSentEmailAsync(deletedSentEmail: entities), times: Times.Once);
        sentEmailProcessingServiceMock.VerifyNoOtherCalls();
        sentEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}