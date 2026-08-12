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
using DataMailReceiver = cCoder.Data.Models.Mail.MailReceiver;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class MailReceiverServiceTests
{
    private readonly Mock<IMailReceiverBroker> mailReceiverBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly MailReceiverService mailReceiverService;

    public MailReceiverServiceTests()
    {
        mailReceiverBrokerMock = new Mock<IMailReceiverBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        mailReceiverService = new MailReceiverService(
mailReceiverBroker: mailReceiverBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static MailReceiver CreateRandomMailReceiver(Guid? id = null, int appId = 7)
    {
        MailReceiver mailReceiver = Builder<MailReceiver>
            .CreateNew()
            .With(func: x => x.Id = id ?? Guid.NewGuid())
            .With(func: x => x.AppId = appId)
            .With(func: x => x.Name = $"MailReceiver-{Guid.NewGuid():N}")
            .With(func: x => x.User = $"user-{Guid.NewGuid():N}")
            .With(func: x => x.Password = $"password-{Guid.NewGuid():N}")
            .With(func: x => x.Host = $"receiver-{Guid.NewGuid():N}.test")
            .With(func: x => x.Port = 25)
            .With(func: x => x.EnableSSL = true)
            .With(func: x => x.LastReceivedOn = DateTimeOffset.UtcNow)
            .With(func: x => x.IsEnabled = true)
            .Build();

        return mailReceiver;
    }

    private static DataMailReceiver ToExternalMailReceiver(MailReceiver item) =>
        item == null
            ? null
            : new DataMailReceiver
            {
                Id = item.Id,
                AppId = item.AppId,
                Name = item.Name,
                ProviderName = item.ProviderName,
                User = item.User,
                Password = item.Password,
                Host = item.Host,
                Port = item.Port,
                EnableSSL = item.EnableSSL,
                LastReceivedOn = item.LastReceivedOn,
                IsEnabled = item.IsEnabled,
                App = item.App == null ? null : new DataApp { Id = item.App.Id, Name = item.App.Name },
            };
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005