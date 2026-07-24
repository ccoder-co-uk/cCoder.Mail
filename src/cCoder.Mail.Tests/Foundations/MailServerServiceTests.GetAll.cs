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

public partial class MailServerServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer();
        IQueryable<cCoder.Data.Models.Mail.MailServer> mailServers = new[] { ToExternalMailServer(item: mailServer) }.AsQueryable();

        mailServerBrokerMock.Setup(expression: x => x.GetAllMailServers(ignoreFilters: false))
            .Returns(value: mailServers);

        // When
        IQueryable<MailServer> result = mailServerService.GetAll();

        // Then

        result.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(expectation: mailServer);

        mailServerBrokerMock.Verify(expression: x => x.GetAllMailServers(ignoreFilters: false), times: Times.Once);
        mailServerBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.MailServer>()), times: Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}