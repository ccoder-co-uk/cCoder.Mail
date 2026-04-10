using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataApp = cCoder.Data.Models.CMS.App;
using DataSentEmail = cCoder.Data.Models.Mail.SentEmail;
using DataUser = cCoder.Data.Models.Security.User;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class SentEmailServiceTests
{
    private readonly Mock<ISentEmailBroker> sentEmailBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly SentEmailService sentEmailService;

    public SentEmailServiceTests()
    {
        sentEmailBrokerMock = new Mock<ISentEmailBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        sentEmailService = new SentEmailService(
            sentEmailBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static SentEmail CreateRandomSentEmail(int id = 42, int appId = 7)
    {
        SentEmail sentEmail = Builder<SentEmail>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.AppId = appId)
            .With(x => x.SentByUserId = $"user-{Guid.NewGuid():N}")
            .With(x => x.Subject = $"Subject-{Guid.NewGuid():N}")
            .With(x => x.Content = "Email body")
            .With(x => x.To = $"to-{Guid.NewGuid():N}@test.local")
            .With(x => x.CC = $"cc-{Guid.NewGuid():N}@test.local")
            .With(x => x.IsBodyHtml = true)
            .With(x => x.SentOn = DateTimeOffset.UtcNow)
            .With(x => x.From = $"from-{Guid.NewGuid():N}@test.local")
            .Build();

        return sentEmail;
    }

    private static DataSentEmail ToExternalSentEmail(SentEmail item) =>
        item == null
            ? null
            : new DataSentEmail
            {
                Id = item.Id,
                AppId = item.AppId,
                SentByUserId = item.SentByUserId,
                Subject = item.Subject,
                Content = item.Content,
                To = item.To,
                CC = item.CC,
                IsBodyHtml = item.IsBodyHtml,
                SentOn = item.SentOn,
                From = item.From,
                App = item.App == null ? null : new DataApp { Id = item.App.Id, Name = item.App.Name },
                SentBy = item.SentBy == null ? null : new DataUser { Id = item.SentBy.Id, DisplayName = item.SentBy.DisplayName, Email = item.SentBy.Email },
            };
}













