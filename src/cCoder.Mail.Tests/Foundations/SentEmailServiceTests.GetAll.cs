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
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail();
        IQueryable<cCoder.Data.Models.Mail.SentEmail> sentEmails = new[] { ToExternalSentEmail(item: sentEmail) }.AsQueryable();

        sentEmailBrokerMock.Setup(expression: x => x.GetAllSentEmails(ignoreFilters: false))
            .Returns(value: sentEmails);

        // When
        IQueryable<SentEmail> result = sentEmailService.GetAll();

        // Then

        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(expectation: sentEmail);

        sentEmailBrokerMock.Verify(expression: x => x.GetAllSentEmails(ignoreFilters: false), times: Times.Once);
        sentEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}