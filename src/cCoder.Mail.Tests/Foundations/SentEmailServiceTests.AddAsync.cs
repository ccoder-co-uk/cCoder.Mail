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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 0, appId: 7);

        cCoder.Data.Models.Mail.SentEmail submitted = null;

        sentEmailBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_create"));

        sentEmailBrokerMock
            .Setup(expression: x =>
                x.AddSentEmailAsync(
entity: It.Is<cCoder.Data.Models.Mail.SentEmail>(match: candidate => !ReferenceEquals(objA: candidate, objB: sentEmail))
                )
            )
            .Callback<cCoder.Data.Models.Mail.SentEmail>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (cCoder.Data.Models.Mail.SentEmail value) => value);

        // When
        SentEmail result = await sentEmailService.AddAsync(sentEmail: sentEmail);

        // Then

        result.Should()
            .BeSameAs(expected: sentEmail);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: sentEmail);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: sentEmail, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: sentEmail, config: options => options.Excluding(expression: candidate => candidate.Id));

        sentEmailBrokerMock.Verify(
expression: x =>
                x.AddSentEmailAsync(
entity: It.Is<cCoder.Data.Models.Mail.SentEmail>(match: candidate => !ReferenceEquals(objA: candidate, objB: sentEmail))
                ),
times: Times.Once
        );

        sentEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        SentEmail sentEmail = CreateRandomSentEmail(id: 0, appId: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await sentEmailService.AddAsync(sentEmail: sentEmail);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        sentEmailBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<cCoder.Data.Models.Mail.SentEmail>()), times: Times.AtMostOnce());
        sentEmailBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "SentEmail_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}