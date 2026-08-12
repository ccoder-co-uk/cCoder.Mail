// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers.Foundations;

public partial class Pop3MailReceiverServiceTests
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
    public async Task ReceiveMailReceiverAsyncShouldMapDependencyExceptions(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        Guid receiverId = Guid.NewGuid();
        storageBrokerMock.Setup(expression: broker => broker.SelectMailReceiverByIdAsync(
                receiverId,
                CancellationToken.None))
            .Throws(exception: dependencyException);

        Func<Task> action = async () =>
            await service.ReceiveMailReceiverAsync(receiverId, 1);

        Exception exception = (await action.Should().ThrowAsync<Exception>()).Which;
        exception.Should().BeOfType(expectedExceptionType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005