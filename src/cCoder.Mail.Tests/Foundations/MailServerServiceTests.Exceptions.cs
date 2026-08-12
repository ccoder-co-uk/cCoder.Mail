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

public partial class MailServerServiceTests
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
    public void GetMailServerShouldMapException(Exception exception, Type expectedType)
    {
        mailServerBrokerMock.Setup(x => x.GetAllMailServers()).Throws(exception);
        Action action = () => mailServerService.GetMailServer(1);
        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task AddMailServerAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        mailServerBrokerMock.Setup(x => x.AddMailServerAsync(It.IsAny<MailServer>())).ThrowsAsync(exception);
        Func<Task> action = async () => await mailServerService.AddMailServerAsync(new MailServer(), false);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task DeleteAllByAppIdAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        mailServerBrokerMock.Setup(x => x.DeleteAllMailServersByAppIdAsync(7)).ThrowsAsync(exception);
        Func<Task> action = async () => await mailServerService.DeleteAllByAppIdAsync(7);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005