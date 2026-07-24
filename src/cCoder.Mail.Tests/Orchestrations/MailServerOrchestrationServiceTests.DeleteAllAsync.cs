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

public partial class MailServerOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        MailServer[] entities = [CreateRandomMailServer()];

        mailServerProcessingServiceMock.Setup(expression: x => x.DeleteAllMailServerAsync(deletedMailServer: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllMailServerAsync(deletedMailServer: entities);

        // Then
        mailServerProcessingServiceMock.Verify(expression: x => x.DeleteAllMailServerAsync(deletedMailServer: entities), times: Times.Once);
        mailServerProcessingServiceMock.VerifyNoOtherCalls();
        mailServerEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}