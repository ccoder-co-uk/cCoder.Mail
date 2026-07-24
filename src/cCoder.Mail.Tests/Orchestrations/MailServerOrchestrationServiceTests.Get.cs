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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        MailServer entity = CreateRandomMailServer();

        mailServerProcessingServiceMock.Setup(expression: x => x.GetMailServer(iMailServerId: id))
            .Returns(value: entity);

        // When
        MailServer result = orchestrationService.GetMailServer(mailServerId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        mailServerProcessingServiceMock.Verify(expression: x => x.GetMailServer(iMailServerId: id), times: Times.Once);
        mailServerProcessingServiceMock.VerifyNoOtherCalls();
        mailServerEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}