// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using System.Security;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Services.Orchestrations;
using cCoder.Mail.Exposures.Controllers;
using cCoder.Mail.Providers.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public sealed partial class SentEmailControllerTests
{
    private readonly Mock<ISentEmailOrchestrationService> serviceMock = new();
    private readonly Mock<ILoggingBroker> loggerMock = new();
    private readonly SentEmailController controller;

    public SentEmailControllerTests()
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
        serviceMock.Setup(x => x.GetAllSentEmail(false)).Returns(new[] { new SentEmail { Id = 1 } }.AsQueryable());
        controller.Get(1).Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetShouldReturnNotFoundWhenEmailDoesNotExist()
    {
        serviceMock.Setup(x => x.GetAllSentEmail(false)).Returns(Array.Empty<SentEmail>().AsQueryable());
        controller.Get(1).Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void GetAllShouldReturnEmails()
    {
        serviceMock.Setup(x => x.GetAllSentEmail(false)).Returns(Array.Empty<SentEmail>().AsQueryable());
        controller.GetAll().Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PostShouldReturnCreatedEmailAsync()
    {
        var email = new SentEmail(); serviceMock.Setup(x => x.AddSentEmailAsync(email)).ReturnsAsync(email);
        ObjectResult result = (ObjectResult)await controller.Post(email);
        result.StatusCode.Should().Be(StatusCodes.Status201Created); result.Value.Should().BeSameAs(email);
    }

    [Fact]
    public async Task PostShouldReturnBadRequestForInvalidModelAsync()
    {
        controller.ModelState.AddModelError("Subject", "Required");
        (await controller.Post(new SentEmail())).Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PutShouldUpdateEmailAsync()
    {
        var email = new SentEmail(); serviceMock.Setup(x => x.UpdateSentEmailAsync(email)).ReturnsAsync(email);
        (await controller.Put(3, email)).Should().BeOfType<OkObjectResult>(); email.Id.Should().Be(3);
    }

    [Fact]
    public async Task PutShouldReturnBadRequestForInvalidModelAsync()
    {
        controller.ModelState.AddModelError("Subject", "Required");
        (await controller.Put(3, new SentEmail())).Should().BeOfType<BadRequestObjectResult>();
    }

}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005