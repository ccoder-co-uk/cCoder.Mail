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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();

        sentEmailProcessingServiceMock.Setup(expression: x => x.AddSentEmailAsync(newSentEmail: entity))
            .ReturnsAsync(value: entity);

        sentEmailEventProcessingServiceMock
            .Setup(expression: x => x.RaiseSentEmailAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        SentEmail result = await orchestrationService.AddSentEmailAsync(newSentEmail: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        sentEmailProcessingServiceMock.Verify(expression: x => x.AddSentEmailAsync(newSentEmail: entity), times: Times.Once);
        sentEmailEventProcessingServiceMock.Verify(expression: x => x.RaiseSentEmailAddEventAsync(entity: entity), times: Times.Once);
    }

}