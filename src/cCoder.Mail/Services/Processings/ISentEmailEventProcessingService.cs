// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;


namespace cCoder.Mail.Services.Processings;

public interface ISentEmailEventProcessingService
{
    ValueTask RaiseSentEmailAddEventAsync(SentEmail entity);
    ValueTask RaiseSentEmailUpdateEventAsync(SentEmail entity);
    ValueTask RaiseSentEmailDeleteEventAsync(SentEmail entity);
}