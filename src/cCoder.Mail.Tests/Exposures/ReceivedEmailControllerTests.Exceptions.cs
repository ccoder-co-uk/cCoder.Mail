// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using System.Security;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class ReceivedEmailControllerTests
{
    public static TheoryData<Exception, int> ControllerExceptionMappings => new()
    {
        { new MailValidationException(new Exception()), StatusCodes.Status400BadRequest },
        { new SecurityException(), StatusCodes.Status403Forbidden },
        { new Exception(), StatusCodes.Status500InternalServerError },
    };

    [Theory]
    [MemberData(nameof(ControllerExceptionMappings))]
    public async Task DeleteShouldMapAndLogExceptionAsync(Exception exception, int statusCode)
    {
        serviceMock.Setup(x => x.DeleteAsync(1)).Throws(exception);
        ((ObjectResult)await controller.Delete(1)).StatusCode.Should().Be(statusCode);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ControllerExceptionMappings))]
    public void GetShouldMapAndLogException(Exception exception, int statusCode)
    {
        serviceMock.Setup(x => x.GetAllReceivedEmail(false)).Throws(exception);
        ((ObjectResult)controller.Get(1)).StatusCode.Should().Be(statusCode);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ControllerExceptionMappings))]
    public void GetAllShouldMapAndLogException(Exception exception, int statusCode)
    {
        serviceMock.Setup(x => x.GetAllReceivedEmail(false)).Throws(exception);
        ((ObjectResult)controller.GetAll()).StatusCode.Should().Be(statusCode);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ControllerExceptionMappings))]
    public async Task PostShouldMapAndLogExceptionAsync(Exception exception, int statusCode)
    {
        serviceMock.Setup(x => x.AddReceivedEmailAsync(It.IsAny<ReceivedEmail>())).Throws(exception);
        ((ObjectResult)await controller.Post(new ReceivedEmail())).StatusCode.Should().Be(statusCode);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ControllerExceptionMappings))]
    public async Task PutShouldMapAndLogExceptionAsync(Exception exception, int statusCode)
    {
        serviceMock.Setup(x => x.UpdateReceivedEmailAsync(It.IsAny<ReceivedEmail>())).Throws(exception);
        ((ObjectResult)await controller.Put(1, new ReceivedEmail())).StatusCode.Should().Be(statusCode);
        loggerMock.Verify(x => x.LogError(exception, "Controller request failed."), Times.Once);
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005