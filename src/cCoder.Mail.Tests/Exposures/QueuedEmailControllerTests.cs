// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using System.Security;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures;
using cCoder.Mail.Exposures.Controllers;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class QueuedEmailControllerTests
{
    private readonly Mock<IQueuedEmailManager> serviceMock = new();
    private readonly Mock<ILoggingBroker> loggerMock = new();
    private readonly QueuedEmailController controller;

    public QueuedEmailControllerTests()
    {
        controller = new(serviceMock.Object, loggerMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task DeleteShouldReturnNoContentAsync()
    {
        serviceMock.Setup(x => x.DeleteAsync(1)).Returns(ValueTask.CompletedTask);
        (await controller.Delete(1)).Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void GetShouldReturnEmail()
    {
        serviceMock.Setup(x => x.GetAllQueuedEmail(false)).Returns(new[] { new QueuedEmail { Id = 1 } }.AsQueryable());
        controller.Get(1).Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetShouldReturnNotFoundWhenEmailDoesNotExist()
    {
        serviceMock.Setup(x => x.GetAllQueuedEmail(false)).Returns(Array.Empty<QueuedEmail>().AsQueryable());
        controller.Get(1).Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void GetAllShouldReturnEmails()
    {
        serviceMock.Setup(x => x.GetAllQueuedEmail(false)).Returns(Array.Empty<QueuedEmail>().AsQueryable());
        controller.GetAll().Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetMetadataShouldReturnMetadata(bool extended)
    {
        controller.Request.QueryString = extended ? new QueryString("?extend=true") : QueryString.Empty;
        controller.GetMetadata().Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PostShouldReturnCreatedEmailAsync()
    {
        var email = new QueuedEmail(); serviceMock.Setup(x => x.AddQueuedEmailAsync(email)).ReturnsAsync(email);
        ObjectResult result = (ObjectResult)await controller.Post(email);
        result.StatusCode.Should().Be(StatusCodes.Status201Created); result.Value.Should().BeSameAs(email);
    }

    [Fact]
    public async Task PostShouldReturnBadRequestForInvalidModelAsync()
    {
        controller.ModelState.AddModelError("Subject", "Required");
        (await controller.Post(new QueuedEmail())).Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PutShouldUpdateEmailAsync()
    {
        var email = new QueuedEmail(); serviceMock.Setup(x => x.UpdateQueuedEmailAsync(email)).ReturnsAsync(email);
        (await controller.Put(3, email)).Should().BeOfType<OkObjectResult>(); email.Id.Should().Be(3);
    }

    [Fact]
    public async Task PutShouldReturnBadRequestForInvalidModelAsync()
    {
        controller.ModelState.AddModelError("Subject", "Required");
        (await controller.Put(3, new QueuedEmail())).Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PatchShouldReturnNotFoundWhenEmailDoesNotExistAsync()
    {
        serviceMock.Setup(x => x.GetQueuedEmail(1)).Returns((QueuedEmail)null);
        (await controller.Put(1, new Delta<QueuedEmail>())).Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PatchShouldUpdateEmailAsync()
    {
        var email = new QueuedEmail { Id = 1 }; serviceMock.Setup(x => x.GetQueuedEmail(1)).Returns(email);
        serviceMock.Setup(x => x.UpdateQueuedEmailAsync(email)).ReturnsAsync(email);
        (await controller.Put(1, new Delta<QueuedEmail>())).Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData(typeof(MailValidationException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(SecurityException), StatusCodes.Status403Forbidden)]
    [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task PatchShouldMapAndLogExceptionAsync(Type exceptionType, int expectedStatus)
    {
        Exception exception = exceptionType == typeof(MailValidationException)
            ? new MailValidationException(new Exception())
            : (Exception)Activator.CreateInstance(exceptionType);
        serviceMock.Setup(x => x.GetQueuedEmail(1)).Throws(exception);
        ObjectResult result = (ObjectResult)await controller.Put(1, new Delta<QueuedEmail>());
        result.StatusCode.Should().Be(expectedStatus);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005