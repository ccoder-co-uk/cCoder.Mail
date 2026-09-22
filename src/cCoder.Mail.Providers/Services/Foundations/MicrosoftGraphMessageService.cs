// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MicrosoftGraphMessageService(
    IMailMessageParsingBroker mailMessageParsingBroker)
    : IMicrosoftGraphMessageService
{
    public MicrosoftGraphMessageEnvelope DeserializeMicrosoftGraphMessages(
        string content) =>
        TryCatch(
            operation: () =>
            {
                ValidateMicrosoftGraphMessagesOnDeserialize(
                    inputs: [content]);

                return mailMessageParsingBroker
                    .DeserializeMicrosoftGraphMessages(
                        content: content);
            });
}