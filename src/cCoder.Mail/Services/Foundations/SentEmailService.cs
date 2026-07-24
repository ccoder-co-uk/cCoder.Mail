// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Mail.Brokers;
using cCoder.Mail.Brokers.Storages;
using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Services.Foundations;

internal class SentEmailService(
    ISentEmailBroker sentEmailBroker,
    IAuthorizationBroker authorizationBroker
) : ISentEmailService
{
    public SentEmail Get(int id)
    {
        SentEmail sentEmail = GetAll()
            .FirstOrDefault(predicate: i => i.Id == id);

        if (sentEmail is not null)
            return sentEmail;

        SentEmail unrestrictedSentEmail = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == id);

        if (unrestrictedSentEmail is not null)
            throw new SecurityException(message: "Access Denied!");

        return null;
    }

    public IQueryable<SentEmail> GetAll(bool ignoreFilters = false) =>
        sentEmailBroker.GetAllSentEmails(ignoreFilters: ignoreFilters);

    public async ValueTask<SentEmail> AddAsync(SentEmail sentEmail)
    {
        authorizationBroker.Authorize(appId: sentEmail.AppId, privilege: $"{nameof(SentEmail)}_create");
        SentEmail result = await sentEmailBroker.AddSentEmailAsync(entity: Copy(sentEmail: sentEmail));
        sentEmail.Id = result.Id;
        sentEmail.AppId = result.AppId;
        sentEmail.SentByUserId = result.SentByUserId;
        sentEmail.Subject = result.Subject;
        sentEmail.Content = result.Content;
        sentEmail.To = result.To;
        sentEmail.CC = result.CC;
        sentEmail.IsBodyHtml = result.IsBodyHtml;
        sentEmail.SentOn = result.SentOn;
        sentEmail.From = result.From;
        sentEmail.MailSenderId = result.MailSenderId;
        return sentEmail;
    }

    public async ValueTask<SentEmail> UpdateAsync(SentEmail sentEmail)
    {
        authorizationBroker.Authorize(appId: sentEmail.AppId, privilege: $"{nameof(SentEmail)}_update");
        SentEmail result = await sentEmailBroker.UpdateSentEmailAsync(entity: Copy(sentEmail: sentEmail));
        sentEmail.Id = result.Id;
        sentEmail.AppId = result.AppId;
        sentEmail.SentByUserId = result.SentByUserId;
        sentEmail.Subject = result.Subject;
        sentEmail.Content = result.Content;
        sentEmail.To = result.To;
        sentEmail.CC = result.CC;
        sentEmail.IsBodyHtml = result.IsBodyHtml;
        sentEmail.SentOn = result.SentOn;
        sentEmail.From = result.From;
        sentEmail.MailSenderId = result.MailSenderId;
        return sentEmail;
    }

    public async ValueTask DeleteAsync(int id)
    {
        SentEmail sentEmail = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == id);

        if (sentEmail is null)
            return;

        authorizationBroker.Authorize(appId: sentEmail.AppId, privilege: $"{nameof(SentEmail)}_delete");
        _ = await sentEmailBroker.DeleteSentEmailAsync(entity: Copy(sentEmail: sentEmail));
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<SentEmail> items) =>
        sentEmailBroker.DeleteAllSentEmailsAsync(
items: items?.Select(selector: Copy) ?? []);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        sentEmailBroker.DeleteAllSentEmailsByAppIdAsync(appId: appId);

    private static SentEmail Copy(SentEmail sentEmail) =>
        sentEmail == null
            ? null
            : new SentEmail
            {
                Id = sentEmail.Id,
                AppId = sentEmail.AppId,
                SentByUserId = sentEmail.SentByUserId,
                Subject = sentEmail.Subject,
                Content = sentEmail.Content,
                To = sentEmail.To,
                CC = sentEmail.CC,
                IsBodyHtml = sentEmail.IsBodyHtml,
                SentOn = sentEmail.SentOn,
                From = sentEmail.From,
                MailSenderId = sentEmail.MailSenderId,
            };
}