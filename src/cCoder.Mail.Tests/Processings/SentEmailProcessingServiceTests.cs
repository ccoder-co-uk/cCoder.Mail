// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;
using cCoder.Mail.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class SentEmailProcessingServiceTests
{
    private DataUser currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ISentEmailService> sentEmailServiceMock = new();
    private readonly SentEmailProcessingService sentEmailProcessingService;

    public SentEmailProcessingServiceTests()
    {
        sentEmailProcessingService = new SentEmailProcessingService(service: sentEmailServiceMock.Object);
    }

    private static SentEmail CreateRandomSentEmail() =>
        Builder<SentEmail>
            .CreateNew()
        .With(func: x => x.Id = Random.Shared.Next(minValue: 1, maxValue: 10000))
        .With(func: x => x.AppId = 1)
        .With(func: x => x.SentByUserId = "test-user")
        .With(func: x => x.Subject = $"Subject-{Guid.NewGuid():N}")
        .With(func: x => x.Content = $"Content-{Guid.NewGuid():N}")
        .With(func: x => x.To = $"{Guid.NewGuid():N}@example.com")
        .With(func: x => x.CC = string.Empty)
        .With(func: x => x.App = null)
        .With(func: x => x.SentBy = null)
        .With(func: x => x.From = "from@example.com")
        .Build();
}