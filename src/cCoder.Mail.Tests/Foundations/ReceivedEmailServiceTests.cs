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
using DataReceivedEmail = cCoder.Data.Models.Mail.ReceivedEmail;
using DataUser = cCoder.Data.Models.Security.User;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class ReceivedEmailServiceTests
{
    private readonly Mock<IReceivedEmailBroker> receivedEmailBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly ReceivedEmailService receivedEmailService;

    public ReceivedEmailServiceTests()
    {
        receivedEmailBrokerMock = new Mock<IReceivedEmailBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        receivedEmailService = new ReceivedEmailService(
receivedEmailBroker: receivedEmailBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static ReceivedEmail CreateRandomReceivedEmail(int id = 42, int appId = 7)
    {
        ReceivedEmail receivedEmail = Builder<ReceivedEmail>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.AppId = appId)
            .With(func: x => x.SentByUserId = $"user-{Guid.NewGuid():N}")
            .With(func: x => x.Subject = $"Subject-{Guid.NewGuid():N}")
            .With(func: x => x.Content = "Email body")
            .With(func: x => x.To = $"to-{Guid.NewGuid():N}@test.local")
            .With(func: x => x.CC = $"cc-{Guid.NewGuid():N}@test.local")
            .With(func: x => x.IsBodyHtml = true)
            .With(func: x => x.ReceivedOn = DateTimeOffset.UtcNow)
            .With(func: x => x.From = $"from-{Guid.NewGuid():N}@test.local")
            .Build();

        return receivedEmail;
    }

    private static DataReceivedEmail ToExternalReceivedEmail(ReceivedEmail item) =>
        item == null
            ? null
            : new DataReceivedEmail
            {
                Id = item.Id,
                AppId = item.AppId,
                SentByUserId = item.SentByUserId,
                Subject = item.Subject,
                Content = item.Content,
                To = item.To,
                CC = item.CC,
                IsBodyHtml = item.IsBodyHtml,
                ReceivedOn = item.ReceivedOn,
                From = item.From,
                MessageId = item.MessageId,
                MailReceiverId = item.MailReceiverId,
                App = item.App == null ? null : new DataApp { Id = item.App.Id, Name = item.App.Name },
                SentBy = item.SentBy == null ? null : new DataUser { Id = item.SentBy.Id, DisplayName = item.SentBy.DisplayName, Email = item.SentBy.Email },
            };
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005