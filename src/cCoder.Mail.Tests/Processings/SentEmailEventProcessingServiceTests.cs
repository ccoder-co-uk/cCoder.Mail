// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations.Events;
using cCoder.Mail.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class SentEmailEventProcessingServiceTests
{
    private readonly Mock<ISentEmailEventService> sentEmailEventServiceMock;
    private readonly SentEmailEventProcessingService service;

    public SentEmailEventProcessingServiceTests()
    {
        sentEmailEventServiceMock = new Mock<ISentEmailEventService>(behavior: MockBehavior.Strict);
        service = new SentEmailEventProcessingService(eventService: sentEmailEventServiceMock.Object);
    }

    private static SentEmail CreateRandomSentEmail() =>
        Builder<SentEmail>.CreateNew()
        .Build();
}