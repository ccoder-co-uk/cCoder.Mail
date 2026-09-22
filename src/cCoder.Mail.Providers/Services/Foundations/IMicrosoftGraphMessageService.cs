// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface IMicrosoftGraphMessageService
{
    MicrosoftGraphMessageEnvelope DeserializeMicrosoftGraphMessages(
        string content);
}