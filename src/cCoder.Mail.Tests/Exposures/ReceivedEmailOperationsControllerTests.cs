// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;
using cCoder.Mail.Exposures.Controllers;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class ReceivedEmailOperationsControllerTests
{
    private readonly Mock<IMailReceivingManager> serviceMock = new();
    private readonly Mock<ILoggingBroker> loggerMock = new();
    private readonly ReceivedEmailOperationsController controller;

    public ReceivedEmailOperationsControllerTests() => controller = new(serviceMock.Object, loggerMock.Object);

    [Fact]
    public async Task PostShouldReturnReceivedEmailsAsync()
    {
        var request = new MailboxReceiveRequest(); ReceivedEmail[] emails = [new() { Id = 1 }];
        serviceMock.Setup(x => x.ReceiveMailboxReceiveRequestAsync(request, CancellationToken.None)).ReturnsAsync(emails);
        ObjectResult result = (ObjectResult)await controller.Post(request, CancellationToken.None);
        result.StatusCode.Should().Be(StatusCodes.Status201Created); result.Value.Should().BeSameAs(emails);
    }

    [Fact]
    public async Task PostShouldReturnBadRequestForInvalidModelAsync()
    {
        controller.ModelState.AddModelError("Provider", "Required");
        (await controller.Post(new MailboxReceiveRequest(), CancellationToken.None)).Should().BeOfType<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData(typeof(ArgumentException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(MailValidationException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task PostShouldMapAndLogExceptionAsync(Type exceptionType, int statusCode)
    {
        Exception exception = exceptionType == typeof(MailValidationException) ? new MailValidationException(new Exception()) : (Exception)Activator.CreateInstance(exceptionType);
        serviceMock.Setup(x => x.ReceiveMailboxReceiveRequestAsync(It.IsAny<MailboxReceiveRequest>(), CancellationToken.None)).ThrowsAsync(exception);
        ((ObjectResult)await controller.Post(new MailboxReceiveRequest(), CancellationToken.None)).StatusCode.Should().Be(statusCode);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }

    [Fact]
    public async Task GetShouldReturnReceivedEmailsAsync()
    {
        Guid id = Guid.NewGuid(); ReceivedEmail[] emails = [new() { Id = 1 }];
        serviceMock.Setup(x => x.ReceiveTopAsync(id, 2, CancellationToken.None)).ReturnsAsync(emails);
        ((OkObjectResult)await controller.Get(id, 2, CancellationToken.None)).Value.Should().BeSameAs(emails);
    }

    [Fact]
    public async Task GetShouldReturnBadRequestForInvalidCountAsync()
    {
        (await controller.Get(Guid.NewGuid(), 0, CancellationToken.None)).Should().BeOfType<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData(typeof(ArgumentException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(MailValidationException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task GetShouldMapAndLogExceptionAsync(Type exceptionType, int statusCode)
    {
        Exception exception = exceptionType == typeof(MailValidationException) ? new MailValidationException(new Exception()) : (Exception)Activator.CreateInstance(exceptionType);
        serviceMock.Setup(x => x.ReceiveTopAsync(It.IsAny<Guid>(), 2, CancellationToken.None)).ThrowsAsync(exception);
        ((ObjectResult)await controller.Get(Guid.NewGuid(), 2, CancellationToken.None)).StatusCode.Should().Be(statusCode);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005