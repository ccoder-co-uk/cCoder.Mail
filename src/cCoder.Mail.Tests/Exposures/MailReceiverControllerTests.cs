// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Brokers.Loggings;
using cCoder.Mail.Exposures.Controllers;
using cCoder.Mail.Exposures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Mail.Tests.Exposures;

public partial class MailReceiverControllerTests
{
    private readonly Mock<IMailReceiverManager> mailReceiverManagerMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();
    private readonly MailReceiverController controller;

    public static TheoryData<Exception, int> FailureExceptions => new()
    {
        { new cCoder.Mail.Providers.Models.Exceptions.MailValidationException(innerException: new Exception()), 400 },
        { new System.Security.SecurityException(), 403 },
        { new Exception(), 500 }
    };

    public MailReceiverControllerTests()
    {
        controller = new MailReceiverController(
            service: mailReceiverManagerMock.Object,
            loggingBroker: loggingBrokerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005