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

public partial class MailReceiverServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        MailReceiver mailReceiver = CreateRandomMailReceiver();
        IQueryable<cCoder.Data.Models.Mail.MailReceiver> mailReceivers = new[] { ToExternalMailReceiver(item: mailReceiver) }.AsQueryable();

        mailReceiverBrokerMock.Setup(expression: x => x.GetAllMailReceivers())
            .Returns(value: mailReceivers);

        // When
        IQueryable<MailReceiver> result = mailReceiverService.GetAllMailReceiver();

        // Then

        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(expectation: mailReceiver);

        mailReceiverBrokerMock.Verify(expression: x => x.GetAllMailReceivers(), times: Times.Once);
        mailReceiverBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailReceiver>()), times: Times.AtMostOnce());
        mailReceiverBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005