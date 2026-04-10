using System.Security;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachEmailWhenUserHasDeletePrivilegeForDeleteAllAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        QueuedEmail email = CreateRandomQueuedEmail();
        DataUser actor = TestUsers.WithPrivilege("queuedemail_delete", email.AppId);
        currentUser = actor;

        queuedEmailServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { email }.AsQueryable());
        queuedEmailServiceMock.Setup(x => x.DeleteAsync(email.Id, false)).Returns(ValueTask.CompletedTask);

        // When
        await queuedEmailProcessingService.DeleteAllAsync([email]);

        // Then
        queuedEmailServiceMock.Verify(x => x.GetAll(true), Times.Once);
        queuedEmailServiceMock.Verify(x => x.DeleteAsync(email.Id, false), Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(email.AppId, "queuedemail_delete"), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenAnEmailIsUnauthorizedForDeleteAllAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        QueuedEmail email = CreateRandomQueuedEmail();
        currentUser = TestUsers.WithoutPrivileges();
        queuedEmailServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { email }.AsQueryable());

        // When
        Func<Task> act = async () => await queuedEmailProcessingService.DeleteAllAsync([email]);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        queuedEmailServiceMock.Verify(x => x.GetAll(true), Times.Once);
        queuedEmailServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(email.AppId, "queuedemail_delete"), Times.Once);
    }
}




