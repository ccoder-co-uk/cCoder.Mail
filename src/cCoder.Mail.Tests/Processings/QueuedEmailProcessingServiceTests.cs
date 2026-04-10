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
            queuedEmailServiceMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static QueuedEmail CreateRandomQueuedEmail() =>
        Builder<QueuedEmail>
            .CreateNew()
            .With(x => x.Id = Random.Shared.Next(1, 10000))
            .With(x => x.AppId = 1)
            .With(x => x.SentByUserId = "test-user")
            .With(x => x.Subject = $"Subject-{Guid.NewGuid():N}")
            .With(x => x.Content = $"Content-{Guid.NewGuid():N}")
            .With(x => x.To = $"{Guid.NewGuid():N}@example.com")
            .With(x => x.CC = string.Empty)
            .With(x => x.App = null)
            .With(x => x.SentBy = null)
            .With(x => x.FailedSends = [])
            .With(x => x.MailServerName = "Default")
            .Build();
}













