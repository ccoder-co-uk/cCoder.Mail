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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();

        mailServerProcessingServiceMock.Setup(expression: x => x.UpdateMailServerAsync(updatedMailServer: entity))
            .ReturnsAsync(value: entity);

        mailServerEventProcessingServiceMock
            .Setup(expression: x => x.RaiseMailServerUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        MailServer result = await orchestrationService.UpdateMailServerAsync(updatedMailServer: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        mailServerProcessingServiceMock.Verify(expression: x => x.UpdateMailServerAsync(updatedMailServer: entity), times: Times.Once);
        mailServerEventProcessingServiceMock.Verify(expression: x => x.RaiseMailServerUpdateEventAsync(entity: entity), times: Times.Once);
    }

}