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
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Mail.Processings;

public partial class QueuedEmailProcessingServiceTests
{
    private DataUser currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IQueuedEmailService> queuedEmailServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly QueuedEmailProcessingService queuedEmailProcessingService;

    public QueuedEmailProcessingServiceTests()
    {
        queuedEmailProcessingService = new QueuedEmailProcessingService(
service: queuedEmailServiceMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static QueuedEmail CreateRandomQueuedEmail() =>
        Builder<QueuedEmail>
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
        .With(func: x => x.FailedSends = [])
        .With(func: x => x.MailServerName = "Default")
        .Build();
}