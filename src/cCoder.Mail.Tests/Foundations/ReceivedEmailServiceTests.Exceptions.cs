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

public partial class ReceivedEmailServiceTests
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
    public void GetReceivedEmailShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        receivedEmailBrokerMock.Setup(expression: broker => broker.GetAllReceivedEmails())
            .Throws(exception: dependencyException);

        Action action = () => receivedEmailService.GetReceivedEmail(receivedEmailId: 7);

        Exception exception = action.Should().Throw<Exception>().Which;
        exception.Should().BeOfType(expectedExceptionType);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task AddReceivedEmailAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        ReceivedEmail receivedEmail = CreateRandomReceivedEmail();
        authorizationBrokerMock.Setup(expression: broker => broker.GetCurrentUser())
            .Throws(exception: dependencyException);

        Func<Task> action = async () =>
            await receivedEmailService.AddReceivedEmailAsync(newReceivedEmail: receivedEmail);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task DeleteAllReceivedEmailAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        ReceivedEmail[] receivedEmails = [CreateRandomReceivedEmail()];
        receivedEmailBrokerMock.Setup(expression: broker =>
                broker.DeleteAllReceivedEmailsAsync(receivedEmails))
            .Throws(exception: dependencyException);

        Func<Task> action = async () =>
            await receivedEmailService.DeleteAllReceivedEmailAsync(deletedReceivedEmail: receivedEmails);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005