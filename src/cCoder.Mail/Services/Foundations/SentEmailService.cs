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

internal partial class SentEmailService(
    ISentEmailBroker sentEmailBroker,
    IAuthorizationBroker authorizationBroker
) : ISentEmailService
{
    public SentEmail GetSentEmail(int sentEmailId) =>
        TryCatch<SentEmail>(operation: () =>
    {

        ValidateSentEmailOnGet(inputs: [sentEmailId]);

        SentEmail sentEmail = GetAllSentEmail()
            .FirstOrDefault(predicate: i => i.Id == sentEmailId);

        if (sentEmail is not null)
        {
            return sentEmail;
        }

        SentEmail unrestrictedSentEmail = GetAllSentEmail(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == sentEmailId);

        if (unrestrictedSentEmail is not null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    });

    public IQueryable<SentEmail> GetAllSentEmail(bool ignoreFilters = false) =>
        TryCatch<IQueryable<SentEmail>>(operation: () =>
        {
            ValidateAllSentEmailOnGet(inputs: [ignoreFilters]);

            return sentEmailBroker.GetAllSentEmails(ignoreFilters: ignoreFilters);
        });

    public ValueTask<SentEmail> AddSentEmailAsync(SentEmail newSentEmail) =>
        TryCatch<SentEmail>(operation: async () =>
    {
        ValidateSentEmailOnAdd(inputs: [newSentEmail]);

        authorizationBroker.Authorize(appId: newSentEmail.AppId, privilege: $"{nameof(SentEmail)}_create");
        SentEmail result = await sentEmailBroker.AddSentEmailAsync(newSentEmail: Copy(sentEmail: newSentEmail));
        newSentEmail.Id = result.Id;
        newSentEmail.AppId = result.AppId;
        newSentEmail.SentByUserId = result.SentByUserId;
        newSentEmail.Subject = result.Subject;
        newSentEmail.Content = result.Content;
        newSentEmail.To = result.To;
        newSentEmail.CC = result.CC;
        newSentEmail.IsBodyHtml = result.IsBodyHtml;
        newSentEmail.SentOn = result.SentOn;
        newSentEmail.From = result.From;
        newSentEmail.MailSenderId = result.MailSenderId;
        return newSentEmail;
    }, isValueTask: true);

    public ValueTask<SentEmail> UpdateSentEmailAsync(SentEmail updatedSentEmail) =>
        TryCatch<SentEmail>(operation: async () =>
    {
        ValidateSentEmailOnUpdate(inputs: [updatedSentEmail]);

        authorizationBroker.Authorize(appId: updatedSentEmail.AppId, privilege: $"{nameof(SentEmail)}_update");
        SentEmail result = await sentEmailBroker.UpdateSentEmailAsync(updatedSentEmail: Copy(sentEmail: updatedSentEmail));
        updatedSentEmail.Id = result.Id;
        updatedSentEmail.AppId = result.AppId;
        updatedSentEmail.SentByUserId = result.SentByUserId;
        updatedSentEmail.Subject = result.Subject;
        updatedSentEmail.Content = result.Content;
        updatedSentEmail.To = result.To;
        updatedSentEmail.CC = result.CC;
        updatedSentEmail.IsBodyHtml = result.IsBodyHtml;
        updatedSentEmail.SentOn = result.SentOn;
        updatedSentEmail.From = result.From;
        updatedSentEmail.MailSenderId = result.MailSenderId;
        return updatedSentEmail;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int sentEmailId) =>
        TryCatch(operation: async () =>
    {

        ValidateDeleteAsync(inputs: [sentEmailId]);

        SentEmail sentEmail = GetAllSentEmail(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == sentEmailId);

        if (sentEmail is null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: sentEmail.AppId, privilege: $"{nameof(SentEmail)}_delete");
        _ = await sentEmailBroker.DeleteSentEmailAsync(deletedSentEmail: Copy(sentEmail: sentEmail));
    }, isValueTask: true);

    public ValueTask DeleteAllForAppSentEmailAsync(IEnumerable<SentEmail> deletedSentEmail) =>
        TryCatch(operation: () =>
    {

        ValidateAllForAppSentEmailOnDelete(inputs: [deletedSentEmail]);

        return sentEmailBroker.DeleteAllSentEmailsAsync(
        deletedSentEmail: deletedSentEmail?.Select(selector: Copy) ?? []);
    }, isValueTask: true);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: () =>
        {
            ValidateAllByAppIdOnDelete(inputs: [appId]);

            return sentEmailBroker.DeleteAllSentEmailsByAppIdAsync(appId: appId);
        }, isValueTask: true);

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