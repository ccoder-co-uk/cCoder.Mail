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

        mailServerBrokerMock.Setup(x => x.GetAllMailServers(false)).Returns(new[] { ToExternalMailServer(mailServer) }.AsQueryable());

        // When
        MailServer result = mailServerService.Get(7);

        // Then
        result.Should().BeEquivalentTo(mailServer);
        mailServerBrokerMock.Verify(x => x.GetAllMailServers(false), Times.Once);
        mailServerBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.MailServer>()), Times.AtMostOnce());
        mailServerBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}










