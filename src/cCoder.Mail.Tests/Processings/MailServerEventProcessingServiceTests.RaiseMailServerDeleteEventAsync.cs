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
    public async Task ShouldPassThroughCallWhenRaiseMailServerDeleteEventAsync()
    {
        // Given
        MailServer entity = CreateRandomMailServer();

        mailServerEventServiceMock
            .Setup(expression: x => x.RaiseMailServerDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseMailServerDeleteEventAsync(entity: entity);

        // Then
        mailServerEventServiceMock.Verify(expression: x => x.RaiseMailServerDeleteEventAsync(entity: entity), times: Times.Once);
        mailServerEventServiceMock.VerifyNoOtherCalls();
    }

}