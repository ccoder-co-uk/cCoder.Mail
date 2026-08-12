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


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class ReceivedEmailServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        ReceivedEmail receivedEmail = CreateRandomReceivedEmail();
        IQueryable<cCoder.Data.Models.Mail.ReceivedEmail> receivedEmails = new[] { ToExternalReceivedEmail(item: receivedEmail) }.AsQueryable();

        receivedEmailBrokerMock.Setup(expression: x => x.GetAllReceivedEmails())
            .Returns(value: receivedEmails);

        // When
        IQueryable<ReceivedEmail> result = receivedEmailService.GetAllReceivedEmail();

        // Then

        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(expectation: receivedEmail);

        receivedEmailBrokerMock.Verify(expression: x => x.GetAllReceivedEmails(), times: Times.Once);
        receivedEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.ReceivedEmail>()), times: Times.AtMostOnce());
        receivedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005