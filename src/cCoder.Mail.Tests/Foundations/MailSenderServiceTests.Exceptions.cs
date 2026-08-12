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

public partial class MailSenderServiceTests
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
    public void GetMailSenderShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        mailSenderBrokerMock.Setup(expression: broker => broker.GetAllMailSenders())
            .Throws(exception: dependencyException);

        Action action = () => mailSenderService.GetMailSender(mailSenderId: Guid.NewGuid());

        Exception exception = action.Should().Throw<Exception>().Which;
        exception.Should().BeOfType(expectedExceptionType);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task AddMailSenderAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        MailSender mailSender = CreateRandomMailSender();
        authorizationBrokerMock.Setup(expression: broker => broker.GetCurrentUser())
            .Throws(exception: dependencyException);

        Func<Task> action = async () =>
            await mailSenderService.AddMailSenderAsync(newMailSender: mailSender);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task DeleteAllMailSenderAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        MailSender[] mailSenders = [CreateRandomMailSender()];
        mailSenderBrokerMock.Setup(expression: broker =>
                broker.DeleteAllMailSendersAsync(mailSenders))
            .Throws(exception: dependencyException);

        Func<Task> action = async () =>
            await mailSenderService.DeleteAllMailSenderAsync(deletedMailSender: mailSenders);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005