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

public partial class MailServerEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseMailServerUpdateEventAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();

        mailServerEventServiceMock
            .Setup(expression: x => x.RaiseMailServerUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseMailServerUpdateEventAsync(entity: entity);

        // Then
        mailServerEventServiceMock.Verify(expression: x => x.RaiseMailServerUpdateEventAsync(entity: entity), times: Times.Once);
        mailServerEventServiceMock.VerifyNoOtherCalls();
    }

}