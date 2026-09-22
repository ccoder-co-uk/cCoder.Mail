// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal interface IMailMessageParsingService
{
    EncodedMailWord[] RetrieveEncodedMailWords(string value);

    string DecodeQuotedPrintable(string content);

    string DecodeBase64(string content);

    string RetrieveMultipartBoundary(string contentType);

}