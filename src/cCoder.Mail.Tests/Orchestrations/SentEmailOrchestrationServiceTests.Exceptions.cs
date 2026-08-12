// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Mail.Orchestrations;

public sealed partial class SentEmailOrchestrationServiceTests
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
    public void GetSentEmailShouldMapException(Exception exception, Type expectedType)
    {
        sentEmailProcessingServiceMock.Setup(x => x.GetSentEmail(1)).Throws(exception);
        Action action = () => orchestrationService.GetSentEmail(1);
        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task AddSentEmailAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        sentEmailProcessingServiceMock.Setup(x => x.AddSentEmailAsync(It.IsAny<SentEmail>())).ThrowsAsync(exception);
        Func<Task> action = async () => await orchestrationService.AddSentEmailAsync(new SentEmail());
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task DeleteByAppIdAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        sentEmailProcessingServiceMock.Setup(x => x.DeleteByAppIdAsync(7)).ThrowsAsync(exception);
        Func<Task> action = async () => await orchestrationService.DeleteByAppIdAsync(7);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005