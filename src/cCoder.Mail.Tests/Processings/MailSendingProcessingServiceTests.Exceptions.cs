// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Processings;

public sealed partial class MailSendingProcessingServiceTests
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
    public void LogDispatchShouldMapException(Exception exception, Type expectedType)
    {
        loggerMock.Setup(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>())).Throws(exception);

        Action action = () => service.LogDispatch(1);

        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task SendQueuedEmailAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        sendingServiceMock.Setup(x => x.SendQueuedEmailAsync(It.IsAny<QueuedEmail>(), CancellationToken.None))
            .ThrowsAsync(exception);

        Func<Task> action = () => service.SendQueuedEmailAsync(new QueuedEmail());

        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005