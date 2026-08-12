// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Services.Orchestrations;

public sealed partial class ReceivedEmailOrchestrationServiceTests
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
        receivedMock.Setup(x => x.GetAllReceivedEmail(true)).Throws(exception);
        Func<Task> action = async () => await service.ExistsAsync(1);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task AddReceivedEmailAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        receivedMock.Setup(x => x.AddReceivedEmailAsync(It.IsAny<ReceivedEmail>())).ThrowsAsync(exception);
        Func<Task> action = async () => await service.AddReceivedEmailAsync(new ReceivedEmail());
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task DeleteByAppIdAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        receivedMock.Setup(x => x.DeleteByAppIdAsync(7)).ThrowsAsync(exception);
        Func<Task> action = async () => await service.DeleteByAppIdAsync(7);
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ReceiveMailboxReceiveRequestAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        receivingMock.Setup(x => x.ReceiveMailboxReceiveRequestAsync(It.IsAny<MailboxReceiveRequest>(), CancellationToken.None))
            .ThrowsAsync(exception);
        Func<Task> action = () => service.ReceiveMailboxReceiveRequestAsync(new MailboxReceiveRequest());
        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005