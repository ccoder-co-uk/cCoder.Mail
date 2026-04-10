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
        IQueryable<cCoder.Data.Models.Mail.MailServer> mailServers = new[] { ToExternalMailServer(mailServer) }.AsQueryable();

        mailServerBrokerMock.Setup(x => x.GetAllMailServers(false)).Returns(mailServers);

        // When
        IQueryable<MailServer> result = mailServerService.GetAll();

        // Then
        result.Should().ContainSingle().Which.Should().BeEquivalentTo(mailServer);
        mailServerBrokerMock.Verify(x => x.GetAllMailServers(false), Times.Once);
        mailServerBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>()), Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









