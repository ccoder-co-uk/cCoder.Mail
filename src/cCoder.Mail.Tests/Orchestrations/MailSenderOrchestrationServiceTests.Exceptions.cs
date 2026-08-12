// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Orchestrations;

public partial class MailSenderOrchestrationServiceTests
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
    public async Task ExistsAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        mailSenderProcessingServiceMock.Setup(x => x.GetAllMailSender(true)).Throws(exception);
        Func<Task> action = async () => await mailSenderOrchestrationService.ExistsAsync(Guid.NewGuid());
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task DeleteByAppIdAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        mailSenderProcessingServiceMock.Setup(x => x.DeleteByAppIdAsync(7)).ThrowsAsync(exception);
        Func<Task> action = async () => await mailSenderOrchestrationService.DeleteByAppIdAsync(7);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task RunAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        queuedEmailProcessingServiceMock.Setup(x => x.GetDispatchBatch(10, 10)).Throws(exception);
        Func<Task> action = () => mailSenderOrchestrationService.RunAsync();
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005