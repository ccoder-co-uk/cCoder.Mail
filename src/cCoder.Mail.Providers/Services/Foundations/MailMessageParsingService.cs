// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MailMessageParsingService(
    IMailMessageParsingBroker mailMessageParsingBroker)
    : IMailMessageParsingService
{
    public EncodedMailWord[] RetrieveEncodedMailWords(string value) =>
        TryCatch(
            operation: () =>
            {
                ValidateEncodedMailWordsOnRetrieve(inputs: [value]);

                return mailMessageParsingBroker.SelectEncodedMailWords(
                    value: value);
            });

    public string DecodeQuotedPrintable(string content) =>
        TryCatch(
            operation: () =>
            {
                ValidateQuotedPrintableOnDecode(inputs: [content]);

                return mailMessageParsingBroker.DecodeQuotedPrintable(
                    content: content);
            });

    public string DecodeBase64(string content) =>
        TryCatch(
            operation: () =>
            {
                ValidateBase64OnDecode(inputs: [content]);

                try
                {
                    return mailMessageParsingBroker.DecodeBase64(
                        content: content);
                }
                catch (FormatException)
                {
                    return content;
                }
            });

    public string RetrieveMultipartBoundary(string contentType) =>
        TryCatch(
            operation: () =>
            {
                ValidateMultipartBoundaryOnRetrieve(inputs: [contentType]);

                return mailMessageParsingBroker.SelectMultipartBoundary(
                    contentType: contentType);
            });
}