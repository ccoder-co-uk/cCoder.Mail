// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataApp = cCoder.Data.Models.CMS.App;
using DataMailServer = cCoder.Data.Models.Mail.MailServer;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailServerServiceTests
{
    private readonly Mock<IMailServerBroker> mailServerBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly MailServerService mailServerService;

    public MailServerServiceTests()
    {
        mailServerBrokerMock = new Mock<IMailServerBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        mailServerService = new MailServerService(
mailServerBroker: mailServerBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static MailServer CreateRandomMailServer(int id = 42, int appId = 7)
    {
        MailServer mailServer = Builder<MailServer>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.AppId = appId)
            .With(func: x => x.Name = $"MailServer-{Guid.NewGuid():N}")
            .With(func: x => x.User = $"user-{Guid.NewGuid():N}")
            .With(func: x => x.Password = $"password-{Guid.NewGuid():N}")
            .With(func: x => x.Host = $"smtp-{Guid.NewGuid():N}.test")
            .With(func: x => x.FromEmail = $"mail-{Guid.NewGuid():N}@test.local")
            .With(func: x => x.Port = 25)
            .With(func: x => x.EnableSSL = true)
            .Build();

        return mailServer;
    }

    private static DataMailServer ToExternalMailServer(MailServer item) =>
        item == null
            ? null
            : new DataMailServer
            {
                Id = item.Id,
                AppId = item.AppId,
                Name = item.Name,
                User = item.User,
                Password = item.Password,
                Host = item.Host,
                FromEmail = item.FromEmail,
                Port = item.Port,
                EnableSSL = item.EnableSSL,
                App = item.App == null ? null : new DataApp { Id = item.App.Id, Name = item.App.Name },
            };
}