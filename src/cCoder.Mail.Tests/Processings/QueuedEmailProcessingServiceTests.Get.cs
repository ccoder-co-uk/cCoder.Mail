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


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        QueuedEmail entity = CreateRandomQueuedEmail();
        var id = entity.Id;

        queuedEmailServiceMock.Setup(expression: x => x.GetQueuedEmail(iQueuedEmailId: id))
            .Returns(value: entity);

        // When
        QueuedEmail result = queuedEmailProcessingService.GetQueuedEmail(queuedEmailId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        queuedEmailServiceMock.Verify(expression: x => x.GetQueuedEmail(iQueuedEmailId: id), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005