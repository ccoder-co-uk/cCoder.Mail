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
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail();
        IQueryable<cCoder.Data.Models.Mail.QueuedEmail> queuedEmails = new[] { ToExternalQueuedEmail(item: queuedEmail) }.AsQueryable();

        queuedEmailBrokerMock.Setup(expression: x => x.GetAllQueuedEmails(ignoreFilters: false))
            .Returns(value: queuedEmails);

        // When
        IQueryable<QueuedEmail> result = queuedEmailService.GetAllQueuedEmail();

        // Then

        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(expectation: queuedEmail);

        queuedEmailBrokerMock.Verify(expression: x => x.GetAllQueuedEmails(ignoreFilters: false), times: Times.Once);
        queuedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), times: Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}