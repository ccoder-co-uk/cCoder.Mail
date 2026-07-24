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
using DataEmailSendFailure = cCoder.Data.Models.Mail.EmailSendFailure;
using DataQueuedEmail = cCoder.Data.Models.Mail.QueuedEmail;
using DataUser = cCoder.Data.Models.Security.User;
using IAuthorizationBroker = cCoder.Mail.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Mail.Foundations;

public partial class QueuedEmailServiceTests
{
    private readonly Mock<IQueuedEmailBroker> queuedEmailBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly QueuedEmailService queuedEmailService;

    public QueuedEmailServiceTests()
    {
        queuedEmailBrokerMock = new Mock<IQueuedEmailBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        queuedEmailService = new QueuedEmailService(
queuedEmailBroker: queuedEmailBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static QueuedEmail CreateRandomQueuedEmail(int id = 42, int appId = 7)
    {
        QueuedEmail queuedEmail = Builder<QueuedEmail>
            .CreateNew()
            .With(func: x => x.Id = id)
            .With(func: x => x.AppId = appId)
            .With(func: x => x.SentByUserId = $"user-{Guid.NewGuid():N}")
            .With(func: x => x.Subject = $"Subject-{Guid.NewGuid():N}")
            .With(func: x => x.Content = "Email body")
            .With(func: x => x.To = $"to-{Guid.NewGuid():N}@test.local")
            .With(func: x => x.CC = $"cc-{Guid.NewGuid():N}@test.local")
            .With(func: x => x.IsBodyHtml = true)
            .With(func: x => x.MailServerName = $"smtp-{Guid.NewGuid():N}")
            .With(func: x => x.MailSenderId = Guid.Parse(input: "00000000-0000-0000-0000-000000000001"))
            .With(func: x => x.MailSender = new MailSender
            {
                Id = Guid.Parse(input: "00000000-0000-0000-0000-000000000001"),
                Name = "Default",
                ProviderName = "SMTP",
            })
            .With(func: x => x.FailedSends = Array.Empty<EmailSendFailure>())
            .Build();

        return queuedEmail;
    }

    private static DataQueuedEmail ToExternalQueuedEmail(QueuedEmail item) =>
        item == null
            ? null
            : new DataQueuedEmail
            {
                Id = item.Id,
                AppId = item.AppId,
                SentByUserId = item.SentByUserId,
                Subject = item.Subject,
                Content = item.Content,
                To = item.To,
                CC = item.CC,
                IsBodyHtml = item.IsBodyHtml,
                MailServerName = item.MailServerName,
                MailSenderId = item.MailSenderId,
                MailSender = item.MailSender == null
                    ? null
                    : new MailSender
                    {
                        Id = item.MailSender.Id,
                        Name = item.MailSender.Name,
                        ProviderName = item.MailSender.ProviderName,
                    },
                App = item.App == null ? null : new DataApp { Id = item.App.Id, Name = item.App.Name },
                SentBy = item.SentBy == null ? null : new DataUser { Id = item.SentBy.Id, DisplayName = item.SentBy.DisplayName, Email = item.SentBy.Email },
                FailedSends = item.FailedSends?.Select(selector: ToExternalEmailSendFailure)
        .ToArray(),
            };

    private static DataEmailSendFailure ToExternalEmailSendFailure(EmailSendFailure item) =>
        item == null
            ? null
            : new DataEmailSendFailure
            {
                Id = item.Id,
                EmailId = item.EmailId,
                AttemptedOn = item.AttemptedOn,
                FailureReason = item.FailureReason,
            };
}