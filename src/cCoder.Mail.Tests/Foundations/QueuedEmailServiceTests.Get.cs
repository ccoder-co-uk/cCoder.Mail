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


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class QueuedEmailServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(id: 7);

        queuedEmailBrokerMock.Setup(expression: x => x.GetAllQueuedEmails(ignoreFilters: false))
            .Returns(value: new[] { ToExternalQueuedEmail(item: queuedEmail) }.AsQueryable());

        // When
        QueuedEmail result = queuedEmailService.Get(id: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: queuedEmail);

        queuedEmailBrokerMock.Verify(expression: x => x.GetAllQueuedEmails(ignoreFilters: false), times: Times.Once);
        queuedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), times: Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}