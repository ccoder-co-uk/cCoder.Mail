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
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        MailReceiver mailReceiver = CreateRandomMailReceiver(id: Guid.Empty);

        mailReceiverBrokerMock.Setup(expression: x => x.GetAllMailReceivers())
            .Returns(value: new[] { ToExternalMailReceiver(item: mailReceiver) }.AsQueryable());

        // When
        MailReceiver result = mailReceiverService.GetMailReceiver(mailReceiverId: Guid.Empty);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: mailReceiver);

        mailReceiverBrokerMock.Verify(expression: x => x.GetAllMailReceivers(), times: Times.Once);
        mailReceiverBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailReceiver>()), times: Times.AtMostOnce());
        mailReceiverBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005