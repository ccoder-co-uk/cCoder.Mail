// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using System;
#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using System.Security;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class QueuedEmailControllerTests
{
    [Fact]
    public async Task PostRetryShouldReturnNoContentAsync()
    {
        serviceMock.Setup(x => x.RetryAsync(1)).Returns(ValueTask.CompletedTask);

        IActionResult result = await controller.Post(1);

        result.Should().BeOfType<NoContentResult>();
        loggerMock.Verify(x => x.LogInformation("Controller request succeeded.", It.IsAny<object[]>()), Times.Once);
    }

    [Theory]
    [InlineData(typeof(MailValidationException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(SecurityException), StatusCodes.Status403Forbidden)]
    [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task PostRetryShouldMapAndLogExceptionAsync(Type exceptionType, int expectedStatus)
    {
        Exception exception = exceptionType == typeof(MailValidationException)
            ? new MailValidationException(new Exception())
            : (Exception)Activator.CreateInstance(exceptionType);
        serviceMock.Setup(x => x.RetryAsync(1)).ThrowsAsync(exception);

        ObjectResult result = (ObjectResult)await controller.Post(1);

        result.StatusCode.Should().Be(expectedStatus);
        if (expectedStatus >= StatusCodes.Status500InternalServerError)
            loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
        else
            loggerMock.Verify(x => x.LogWarning(exception.Message, It.IsAny<object[]>()), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005