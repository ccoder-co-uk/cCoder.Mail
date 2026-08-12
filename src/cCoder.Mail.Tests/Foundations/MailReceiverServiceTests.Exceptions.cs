// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailReceiverServiceTests
{
    public static TheoryData<Exception, Type> DependencyExceptions => new()
    {
        { new MailValidationException(innerException: new Exception()), typeof(MailValidationException) },
        { new MailDependencyException(innerException: new Exception()), typeof(MailDependencyException) },
        { new ArgumentException(), typeof(MailValidationException) },
        { new Exception(), typeof(MailServiceException) }
    };

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public void GetMailReceiverShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        mailReceiverBrokerMock.Setup(expression: broker => broker.GetAllMailReceivers())
            .Throws(exception: dependencyException);

        Action action = () => mailReceiverService.GetMailReceiver(mailReceiverId: Guid.NewGuid());

        Exception exception = action.Should().Throw<Exception>().Which;
        exception.Should().BeOfType(expectedExceptionType);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task AddMailReceiverAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        MailReceiver mailReceiver = CreateRandomMailReceiver();
        authorizationBrokerMock.Setup(expression: broker => broker.GetCurrentUser())
            .Throws(exception: dependencyException);

        Func<Task> action = async () =>
            await mailReceiverService.AddMailReceiverAsync(newMailReceiver: mailReceiver);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task DeleteAllMailReceiverAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        MailReceiver[] mailReceivers = [CreateRandomMailReceiver()];
        mailReceiverBrokerMock.Setup(expression: broker =>
                broker.DeleteAllMailReceiversAsync(mailReceivers))
            .Throws(exception: dependencyException);

        Func<Task> action = async () =>
            await mailReceiverService.DeleteAllMailReceiverAsync(deletedMailReceiver: mailReceivers);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005