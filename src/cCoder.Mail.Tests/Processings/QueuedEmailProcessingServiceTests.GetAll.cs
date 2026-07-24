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


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<QueuedEmail> entities = new[] { CreateRandomQueuedEmail() }.AsQueryable();

        queuedEmailServiceMock.Setup(expression: x => x.GetAllQueuedEmail())
            .Returns(value: entities);

        // When
        IQueryable<QueuedEmail> result = queuedEmailProcessingService.GetAllQueuedEmail();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        queuedEmailServiceMock.Verify(expression: x => x.GetAllQueuedEmail(), times: Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
    }

}