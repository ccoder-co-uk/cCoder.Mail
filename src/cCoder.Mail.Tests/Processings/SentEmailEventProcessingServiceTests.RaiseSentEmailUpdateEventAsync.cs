// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class SentEmailEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseSentEmailUpdateEventAsync()
    {
        // Given
        SentEmail entity = CreateRandomSentEmail();

        sentEmailEventServiceMock
            .Setup(expression: x => x.RaiseSentEmailUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseSentEmailUpdateEventAsync(entity: entity);

        // Then
        sentEmailEventServiceMock.Verify(expression: x => x.RaiseSentEmailUpdateEventAsync(entity: entity), times: Times.Once);
        sentEmailEventServiceMock.VerifyNoOtherCalls();
    }

}