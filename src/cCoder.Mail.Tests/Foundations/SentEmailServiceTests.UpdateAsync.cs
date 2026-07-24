// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 7);

        cCoder.Data.Models.Mail.SentEmail submitted = null;

        sentEmailBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_update"));

        sentEmailBrokerMock
            .Setup(expression: x => x.UpdateSentEmailAsync(updatedSentEmail: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()))
            .Callback<cCoder.Data.Models.Mail.SentEmail>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.SentEmail value) => value);

        // When
        SentEmail result = await sentEmailService.UpdateSentEmailAsync(updatedSentEmail: sentEmail);

        // Then

        result.Should()
            .BeSameAs(expected: sentEmail);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: sentEmail);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: sentEmail);

        result.Should()
            .BeEquivalentTo(expectation: sentEmail);

        sentEmailBrokerMock.Verify(expression: x => x.UpdateSentEmailAsync(updatedSentEmail: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.Once);
        sentEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await sentEmailService.UpdateSentEmailAsync(updatedSentEmail: sentEmail);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        sentEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}