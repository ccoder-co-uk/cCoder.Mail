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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();

        mailServerProcessingServiceMock.Setup(expression: x => x.AddMailServerAsync(newMailServer: entity))
            .ReturnsAsync(value: entity);

        mailServerEventProcessingServiceMock
            .Setup(expression: x => x.RaiseMailServerAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        MailServer result = await orchestrationService.AddMailServerAsync(newMailServer: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        mailServerProcessingServiceMock.Verify(expression: x => x.AddMailServerAsync(newMailServer: entity), times: Times.Once);
        mailServerEventProcessingServiceMock.Verify(expression: x => x.RaiseMailServerAddEventAsync(entity: entity), times: Times.Once);
    }

}