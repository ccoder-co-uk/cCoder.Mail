// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Processings;

public sealed partial class MailReceivingProcessingServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings => new()
    {
        { new MailValidationException(new Exception()), typeof(MailValidationException) },
        { new MailDependencyException(new Exception()), typeof(MailDependencyException) },
        { new ArgumentException("invalid"), typeof(MailValidationException) },
        { new InvalidOperationException("failed"), typeof(MailServiceException) },
    };

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void IsMigrationInProgressShouldMapException(Exception exception, Type expectedType)
    {
        configurationMock.Setup(x => x.GetMailConfiguration()).Throws(exception);
        Action action = () => service.IsMigrationInProgress();
        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void LogErrorShouldMapException(Exception exception, Type expectedType)
    {
        loggerMock.Setup(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>())).Throws(exception);
        Action action = () => service.LogError(new Exception("logged"));
        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ReceiveMailboxReceiveRequestAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        receivingServiceMock.Setup(x => x.ReceiveMailboxReceiveRequestAsync(It.IsAny<MailboxReceiveRequest>(), CancellationToken.None))
            .ThrowsAsync(exception);
        Func<Task> action = () => service.ReceiveMailboxReceiveRequestAsync(new MailboxReceiveRequest());
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005