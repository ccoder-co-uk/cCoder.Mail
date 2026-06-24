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
        SentEmail sentEmail = GetAll().FirstOrDefault(i => i.Id == id);
        if (sentEmail is not null)
            return sentEmail;

        SentEmail unrestrictedSentEmail = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedSentEmail is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<SentEmail> GetAll(bool ignoreFilters = false) =>
        sentEmailBroker.GetAllSentEmails(ignoreFilters);

    public async ValueTask<SentEmail> AddAsync(SentEmail sentEmail)
    {
        authorizationBroker.Authorize(sentEmail.AppId, $"{nameof(SentEmail)}_create");
        SentEmail result = await sentEmailBroker.AddSentEmailAsync(Copy(sentEmail));
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
        return sentEmail;
    }

    public async ValueTask<SentEmail> UpdateAsync(SentEmail sentEmail)
    {
        authorizationBroker.Authorize(sentEmail.AppId, $"{nameof(SentEmail)}_update");
        SentEmail result = await sentEmailBroker.UpdateSentEmailAsync(Copy(sentEmail));
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
        return sentEmail;
    }

    public async ValueTask DeleteAsync(int id)
    {
        SentEmail sentEmail = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (sentEmail is null)
            return;

        authorizationBroker.Authorize(sentEmail.AppId, $"{nameof(SentEmail)}_delete");
        _ = await sentEmailBroker.DeleteSentEmailAsync(Copy(sentEmail));
    }

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
            };
}


