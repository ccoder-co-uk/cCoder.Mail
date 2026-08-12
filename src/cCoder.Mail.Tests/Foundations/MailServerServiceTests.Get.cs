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

public partial class MailServerServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer(id: 7);

        mailServerBrokerMock.Setup(expression: x => x.GetAllMailServers())
            .Returns(value: new[] { ToExternalMailServer(item: mailServer) }.AsQueryable());

        // When
        MailServer result = mailServerService.GetMailServer(mailServerId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: mailServer);

        mailServerBrokerMock.Verify(expression: x => x.GetAllMailServers(), times: Times.Once);
        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005