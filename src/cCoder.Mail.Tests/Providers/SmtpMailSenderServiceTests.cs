// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Models.Exceptions;
using cCoder.Mail.Providers.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Providers;

public sealed partial class SmtpMailSenderServiceTests
{
    private readonly Mock<ISmtpMailSenderBroker> brokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings => new()
    {
        { new MailValidationException(new Exception()), typeof(MailValidationException) },
        { new MailDependencyException(new Exception()), typeof(MailDependencyException) },
        { new ArgumentException("invalid"), typeof(MailValidationException) },
        { new InvalidOperationException("failed"), typeof(MailServiceException) },
    };

    [Fact]
    public async Task SendQueuedEmailAsyncShouldDelegateAsync()
    {
        var email = new QueuedEmail();
        brokerMock.Setup(x => x.SendAsync(email, CancellationToken.None)).Returns(Task.CompletedTask);
        var service = new SmtpMailSenderService(brokerMock.Object);

        await service.SendQueuedEmailAsync(email);

        brokerMock.VerifyAll();
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task SendQueuedEmailAsyncShouldMapExceptionAsync(Exception exception, Type expectedType)
    {
        brokerMock.Setup(x => x.SendAsync(It.IsAny<QueuedEmail>(), CancellationToken.None)).ThrowsAsync(exception);
        var service = new SmtpMailSenderService(brokerMock.Object);

        Func<Task> action = () => service.SendQueuedEmailAsync(new QueuedEmail());

        (await action.Should().ThrowAsync<Exception>()).Which.Should().BeOfType(expectedType);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005