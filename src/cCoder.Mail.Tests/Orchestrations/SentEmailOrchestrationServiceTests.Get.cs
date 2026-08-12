// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        SentEmail entity = CreateRandomSentEmail();

        sentEmailProcessingServiceMock.Setup(expression: x => x.GetSentEmail(iSentEmailId: id))
            .Returns(value: entity);

        // When
        SentEmail result = orchestrationService.GetSentEmail(sentEmailId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        sentEmailProcessingServiceMock.Verify(expression: x => x.GetSentEmail(iSentEmailId: id), times: Times.Once);
        sentEmailProcessingServiceMock.VerifyNoOtherCalls();
        sentEmailEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005