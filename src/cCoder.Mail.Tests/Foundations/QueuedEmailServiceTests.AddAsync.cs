using System.Security;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class QueuedEmailServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(id: 0, appId: 7);

        cCoder.Data.Models.Mail.QueuedEmail submitted = null;

        queuedEmailBrokerMock.Setup(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "QueuedEmail_create"));

        queuedEmailBrokerMock
            .Setup(x =>
                x.AddQueuedEmailAsync(
                    It.Is<cCoder.Data.Models.Mail.QueuedEmail>(candidate => !ReferenceEquals(candidate, queuedEmail))
                )
            )
            .Callback<cCoder.Data.Models.Mail.QueuedEmail>(candidate => submitted = candidate)
            .ReturnsAsync((cCoder.Data.Models.Mail.QueuedEmail value) => value);

        // When
        QueuedEmail result = await queuedEmailService.AddAsync(queuedEmail);

        // Then
        result.Should().NotBeSameAs(queuedEmail);
        submitted.Should().NotBeNull();

        submitted
            .Should()
            .BeEquivalentTo(
                queuedEmail,
                options => options.Excluding(candidate => candidate.Id).Excluding(candidate => candidate.FailedSends)
            );

        result
            .Should()
            .BeEquivalentTo(queuedEmail, options => options.Excluding(candidate => candidate.Id));

        queuedEmailBrokerMock.Verify(
            x =>
                x.AddQueuedEmailAsync(
                    It.Is<cCoder.Data.Models.Mail.QueuedEmail>(candidate => !ReferenceEquals(candidate, queuedEmail))
                ),
            Times.Once
        );
        queuedEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "QueuedEmail_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        QueuedEmail queuedEmail = CreateRandomQueuedEmail(id: 0, appId: 7);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "QueuedEmail_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await queuedEmailService.AddAsync(queuedEmail);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        queuedEmailBrokerMock.Verify(x => x.GetAppId(It.IsAny<cCoder.Data.Models.Mail.QueuedEmail>()), Times.AtMostOnce());
        queuedEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "QueuedEmail_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









