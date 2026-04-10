using System.Security;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class MailServerProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer();
        mailServerServiceMock.Setup(x => x.AddAsync(mailServer)).ReturnsAsync(mailServer);

        // When
        MailServer result = await mailServerProcessingService.AddAsync(mailServer);

        // Then
        Assert.Same(mailServer, result);
        mailServerServiceMock.Verify(x => x.AddAsync(mailServer), Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        MailServer mailServer = CreateRandomMailServer();

        mailServerServiceMock
            .Setup(x => x.AddAsync(mailServer))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await mailServerProcessingService.AddAsync(mailServer)
        );

        // Then
    }

}









