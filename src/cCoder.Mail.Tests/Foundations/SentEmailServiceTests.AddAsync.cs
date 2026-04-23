using System.Security;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class SentEmailServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 0, appId: 7);

        cCoder.Data.Models.Mail.SentEmail submitted = null;

        sentEmailBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "SentEmail_create"));

        sentEmailBrokerMock
            .Setup(x =>
                x.AddSentEmailAsync(
                    It.Is<cCoder.Data.Models.Mail.SentEmail>(candidate => !ReferenceEquals(candidate, sentEmail))
                )
            )
            .Callback<cCoder.Data.Models.Mail.SentEmail>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Mail.SentEmail value) => value);

        // When
        SentEmail result = await sentEmailService.AddAsync(sentEmail);

        // Then
        result.Should().BeSameAs(sentEmail);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(sentEmail);
        result.Should().NotBeSameAs(submitted);

        submitted
            .Should()
            .BeEquivalentTo(sentEmail, options => options.Excluding(candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(sentEmail, options => options.Excluding(candidate => candidate.Id));

        sentEmailBrokerMock.Verify(
            x =>
                x.AddSentEmailAsync(
                    It.Is<cCoder.Data.Models.Mail.SentEmail>(candidate => !ReferenceEquals(candidate, sentEmail))
                ),
            Times.Once
        );
        sentEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "SentEmail_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 0, appId: 7);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "SentEmail_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await sentEmailService.AddAsync(sentEmail);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        sentEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "SentEmail_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









