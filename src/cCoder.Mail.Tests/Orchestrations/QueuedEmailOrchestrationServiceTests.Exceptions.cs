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

public sealed partial class QueuedEmailOrchestrationServiceTests
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
    public void GetQueuedEmailShouldMapException(Exception exception, Type expectedType)
    {
        queuedEmailProcessingServiceMock.Setup(x => x.GetQueuedEmail(1)).Throws(exception);
        Action action = () => orchestrationService.GetQueuedEmail(1);
        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task AddQueuedEmailAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        queuedEmailProcessingServiceMock.Setup(x => x.AddQueuedEmailAsync(It.IsAny<QueuedEmail>())).ThrowsAsync(exception);
        Func<Task> action = async () => await orchestrationService.AddQueuedEmailAsync(new QueuedEmail());
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task DeleteByAppIdAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        queuedEmailProcessingServiceMock.Setup(x => x.DeleteByAppIdAsync(7)).ThrowsAsync(exception);
        Func<Task> action = async () => await orchestrationService.DeleteByAppIdAsync(7);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005