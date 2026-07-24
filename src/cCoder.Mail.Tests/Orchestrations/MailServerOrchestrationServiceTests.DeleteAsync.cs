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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        int id = 1;
        MailServer entity = CreateRandomMailServer();

        mailServerProcessingServiceMock.Setup(expression: x => x.GetAllMailServer(ignoreFilters: true))
            .Returns(value: new[] { entity }.AsQueryable());

        mailServerProcessingServiceMock.Setup(expression: x => x.DeleteAsync(iMailServerId: id))
            .Returns(value: ValueTask.CompletedTask);

        mailServerEventProcessingServiceMock
            .Setup(expression: x => x.RaiseMailServerDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(mailServerId: id);

        // Then
        mailServerProcessingServiceMock.Verify(expression: x => x.GetAllMailServer(ignoreFilters: true), times: Times.Once);
        mailServerProcessingServiceMock.Verify(expression: x => x.DeleteAsync(iMailServerId: id), times: Times.Once);
        mailServerEventProcessingServiceMock.Verify(expression: x => x.RaiseMailServerDeleteEventAsync(entity: entity), times: Times.Once);
    }

}