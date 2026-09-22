// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSendingServiceTests
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
    public void IsMigrationInProgressShouldMapException(
        Exception exception,
        Type expectedType)
    {
        mailConfigurationBrokerMock
            .Setup(expression: broker => broker.GetMailConfiguration())
            .Throws(exception: exception);

        Action action = () => mailSendingService.IsMigrationInProgress();

        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void LogDispatchShouldMapException(
        Exception exception,
        Type expectedType)
    {
        loggerMock
            .Setup(expression: broker => broker.LogInformation(
                It.IsAny<string>(),
                It.IsAny<object[]>()))
            .Throws(exception: exception);

        Action action = () => mailSendingService.LogDispatch(count: 1);

        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005