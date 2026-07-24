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

public partial class SentEmailServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 7);

        sentEmailBrokerMock.Setup(expression: x => x.GetAllSentEmails(ignoreFilters: false))
            .Returns(value: new[] { ToExternalSentEmail(item: sentEmail) }.AsQueryable());

        // When
        SentEmail result = sentEmailService.GetSentEmail(sentEmailId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: sentEmail);

        sentEmailBrokerMock.Verify(expression: x => x.GetAllSentEmails(ignoreFilters: false), times: Times.Once);
        sentEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}