// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal interface IMailMessageParsingBroker
{
    EncodedMailWord[] SelectEncodedMailWords(string value);

    string DecodeQuotedPrintable(string content);

    string DecodeBase64(string content);

    string SelectMultipartBoundary(string contentType);

    MicrosoftGraphMessageEnvelope DeserializeMicrosoftGraphMessages(
        string content);
}