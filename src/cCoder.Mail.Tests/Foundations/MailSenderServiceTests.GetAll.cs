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

public partial class MailSenderServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        MailSender mailSender = CreateRandomMailSender();
        IQueryable<cCoder.Data.Models.Mail.MailSender> mailSenders = new[] { ToExternalMailSender(item: mailSender) }.AsQueryable();

        mailSenderBrokerMock.Setup(expression: x => x.GetAllMailSenders())
            .Returns(value: mailSenders);

        // When
        IQueryable<MailSender> result = mailSenderService.GetAllMailSender();

        // Then

        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(expectation: mailSender);

        mailSenderBrokerMock.Verify(expression: x => x.GetAllMailSenders(), times: Times.Once);
        mailSenderBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailSender>()), times: Times.AtMostOnce());
        mailSenderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005