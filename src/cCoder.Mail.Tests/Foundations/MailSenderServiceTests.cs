// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;
using cCoder.Mail.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataApp = cCoder.Data.Models.CMS.App;
using DataMailSender = cCoder.Data.Models.Mail.MailSender;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailSenderServiceTests
{
    private readonly Mock<IMailSenderBroker> mailSenderBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly MailSenderService mailSenderService;

    public MailSenderServiceTests()
    {
        mailSenderBrokerMock = new Mock<IMailSenderBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        mailSenderService = new MailSenderService(
mailSenderBroker: mailSenderBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static MailSender CreateRandomMailSender(Guid? id = null, int appId = 7)
    {
        MailSender mailSender = Builder<MailSender>
            .CreateNew()
            .With(func: x => x.Id = id ?? Guid.NewGuid())
            .With(func: x => x.AppId = appId)
            .With(func: x => x.Name = $"MailSender-{Guid.NewGuid():N}")
            .With(func: x => x.User = $"user-{Guid.NewGuid():N}")
            .With(func: x => x.Password = $"password-{Guid.NewGuid():N}")
            .With(func: x => x.Host = $"sender-{Guid.NewGuid():N}.test")
            .With(func: x => x.FromEmail = $"mail-{Guid.NewGuid():N}@test.local")
            .With(func: x => x.Port = 25)
            .With(func: x => x.EnableSSL = true)
            .Build();

        return mailSender;
    }

    private static DataMailSender ToExternalMailSender(MailSender item) =>
        item == null
            ? null
            : new DataMailSender
            {
                Id = item.Id,
                AppId = item.AppId,
                Name = item.Name,
                ProviderName = item.ProviderName,
                User = item.User,
                Password = item.Password,
                Host = item.Host,
                FromEmail = item.FromEmail,
                Port = item.Port,
                EnableSSL = item.EnableSSL,
                App = item.App == null ? null : new DataApp { Id = item.App.Id, Name = item.App.Name },
            };
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005