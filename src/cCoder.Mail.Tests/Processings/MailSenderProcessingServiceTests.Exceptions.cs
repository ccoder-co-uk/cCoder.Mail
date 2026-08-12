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

public sealed partial class MailSenderProcessingServiceTests
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
    public void GetMailSenderShouldMapException(Exception exception, Type expectedType)
    {
        serviceMock.Setup(x => x.GetMailSender(It.IsAny<Guid>())).Throws(exception);
        Action action = () => service.GetMailSender(Guid.NewGuid());
        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task AddMailSenderAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        serviceMock.Setup(x => x.AddMailSenderAsync(It.IsAny<MailSender>())).ThrowsAsync(exception);
        Func<Task> action = async () => await service.AddMailSenderAsync(new MailSender());
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task DeleteByAppIdAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        serviceMock.Setup(x => x.DeleteAllByAppIdAsync(7)).ThrowsAsync(exception);
        Func<Task> action = async () => await service.DeleteByAppIdAsync(7);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005