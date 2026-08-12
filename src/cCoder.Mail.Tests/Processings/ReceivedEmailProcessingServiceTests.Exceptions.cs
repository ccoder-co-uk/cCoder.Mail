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

public sealed partial class ReceivedEmailProcessingServiceTests
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
    public void GetReceivedEmailShouldMapException(Exception exception, Type expectedType)
    {
        foundationMock.Setup(x => x.GetReceivedEmail(1)).Throws(exception);
        Action action = () => service.GetReceivedEmail(1);
        action.Should().Throw<Exception>().Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task AddReceivedEmailAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        foundationMock.Setup(x => x.AddReceivedEmailAsync(It.IsAny<ReceivedEmail>())).ThrowsAsync(exception);
        Func<Task> action = async () => await service.AddReceivedEmailAsync(new ReceivedEmail());
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task DeleteByAppIdAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        foundationMock.Setup(x => x.DeleteAllByAppIdAsync(7)).ThrowsAsync(exception);
        Func<Task> action = async () => await service.DeleteByAppIdAsync(7);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005