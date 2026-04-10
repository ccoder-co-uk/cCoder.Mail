using System.Security;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class SentEmailProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail();
        sentEmailServiceMock.Setup(x => x.AddAsync(sentEmail)).ReturnsAsync(sentEmail);

        // When
        SentEmail result = await sentEmailProcessingService.AddAsync(sentEmail);

        // Then
        Assert.Same(sentEmail, result);
        sentEmailServiceMock.Verify(x => x.AddAsync(sentEmail), Times.Once);
    }

    [Fact]
    public async Task ShouldPropagateSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail();

        sentEmailServiceMock
            .Setup(x => x.AddAsync(sentEmail))
            .ThrowsAsync(new SecurityException("Access Denied!"));

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await sentEmailProcessingService.AddAsync(sentEmail)
        );

        // Then
    }

}









