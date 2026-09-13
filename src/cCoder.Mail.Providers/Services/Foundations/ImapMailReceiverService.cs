// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class ImapMailReceiverService(
    IImapMailReceiverBroker imapMailReceiverBroker,
    IMailReceiverStorageBroker mailReceiverStorageBroker,
    IMailMessageParsingBroker mailMessageParsingBroker)
    : IImapMailReceiverService
{
    public Task<ReceivedEmail[]> ReceiveMailReceiverAsync(
        Guid mailReceiverId,
        int maximumMessages,
        CancellationToken cancellationToken = default) =>
        TryCatch<ReceivedEmail[]>(operation: async () =>
        {
            ValidateReceiveMailReceiverAsync(
                inputs:
                    [
                        mailReceiverId,
                        maximumMessages,
                        cancellationToken
                    ]);

            MailReceiver mailReceiver =
                await mailReceiverStorageBroker
                    .SelectMailReceiverByIdAsync(
                        mailReceiverId: mailReceiverId,
                        cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException(
                    message:
                        $"Mail receiver '{mailReceiverId}' was not found.");

            return await ReceiveMailboxAsync(
                request: CreateMailboxReceiveRequest(
                    mailReceiver: mailReceiver,
                    maximumMessages: maximumMessages),
                cancellationToken: cancellationToken);
        }, isTask: true);

    private static MailboxReceiveRequest CreateMailboxReceiveRequest(
        MailReceiver mailReceiver,
        int maximumMessages) =>
        new()
        {
            ProviderName = mailReceiver.ProviderName,
            AppId = mailReceiver.AppId,
            MailReceiverId = mailReceiver.Id,
            Host = mailReceiver.Host,
            Port = mailReceiver.Port,
            EnableSSL = mailReceiver.EnableSSL,
            User = mailReceiver.User,
            Password = mailReceiver.Password,
            MaximumMessages = maximumMessages,
        };

    private async Task<ReceivedEmail[]> ReceiveMailboxAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken)
    {
        ValidateReceiveRequest(request: request);

        string[] rawMessages = await imapMailReceiverBroker.ReceiveAsync(
            request: request,
            cancellationToken: cancellationToken);

        return
        [
            .. rawMessages
                .Select(selector: ParseMessage)
                .OrderByDescending(
                    keySelector: message => message.ReceivedOn)
        ];
    }

    private ReceivedEmail ParseMessage(string rawMessage)
    {
        string[] lines = rawMessage.Split(separator: '\n')
            .Select(selector: line => line.TrimEnd(trimChar: '\r'))
            .ToArray();

        int headerStart = Array.FindIndex(array: lines, match: line => line.StartsWith(value: "From:", comparisonType: StringComparison.OrdinalIgnoreCase));

        if (headerStart > 0)
        {
            lines = lines[headerStart..];
        }

        int separatorIndex = Array.FindIndex(array: lines, match: string.IsNullOrWhiteSpace);
        string[] headerLines = separatorIndex >= 0 ? lines[..separatorIndex] : lines;
        string[] bodyLines = separatorIndex >= 0 ? lines[(separatorIndex + 1)..] : [];
        Dictionary<string, string> headers = ParseHeaders(lines: headerLines);

        return new()
        {
            MessageId = Header(headers: headers, name: "Message-ID"),
            From = Header(headers: headers, name: "From"),
            To = Header(headers: headers, name: "To"),
            CC = Header(headers: headers, name: "Cc"),
            Subject = DecodeHeader(value: Header(headers: headers, name: "Subject")),
            Content = string.Join(separator: "\n", value: bodyLines)
            .TrimEnd(trimChars: [')', '\r', '\n']),
            IsBodyHtml = Header(headers: headers, name: "Content-Type")?.StartsWith(value: "text/html", comparisonType: StringComparison.OrdinalIgnoreCase) == true,
            ReceivedOn = ParseDate(value: Header(headers: headers, name: "Date")),
        };
    }

    private static Dictionary<string, string> ParseHeaders(string[] lines)
    {
        Dictionary<string, string> headers = new(comparer: StringComparer.OrdinalIgnoreCase);
        string currentName = null;

        foreach (string line in lines)
        {
            if ((line.StartsWith(value: ' ') || line.StartsWith(value: '\t')) && currentName != null)
            {
                headers[currentName] += " " + line.Trim();
                continue;
            }

            int separatorIndex = line.IndexOf(value: ':');

            if (separatorIndex <= 0)
            {
                continue;
            }

            currentName = line[..separatorIndex];
            headers[currentName] = line[(separatorIndex + 1)..].Trim();
        }

        return headers;
    }

    private static string Header(Dictionary<string, string> headers, string name) =>
        headers.TryGetValue(key: name, value: out string value) ? value : null;

    private string DecodeHeader(string value)
    {
        string decoded = value;

        foreach (EncodedMailWord word in mailMessageParsingBroker
            .SelectEncodedMailWords(value: value ?? string.Empty))
        {
            string replacement = string.Equals(
                a: word.Encoding,
                b: "B",
                comparisonType: StringComparison.OrdinalIgnoreCase)
                    ? Encoding.UTF8.GetString(
                        bytes: Convert.FromBase64String(s: word.Text))
                    : word.Text.Replace(oldChar: '_', newChar: ' ');

            decoded = decoded.Replace(
                oldValue: word.Value,
                newValue: replacement);
        }

        return decoded;
    }

    private static DateTimeOffset ParseDate(string value) =>
        DateTimeOffset.TryParse(input: value, result: out DateTimeOffset parsed)
            ? parsed
            : DateTimeOffset.MinValue;

    private static void ValidateReceiveRequest(MailboxReceiveRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(paramName: nameof(request));
        }

        if (string.IsNullOrWhiteSpace(value: request.Host))
        {
            throw new InvalidOperationException(message: "Mailbox host is required.");
        }

        if (request.Port <= 0)
        {
            throw new InvalidOperationException(message: "Mailbox port is required.");
        }

        if (string.IsNullOrWhiteSpace(value: request.User))
        {
            throw new InvalidOperationException(message: "Mailbox user is required.");
        }

        if (string.IsNullOrWhiteSpace(value: request.Password))
        {
            throw new InvalidOperationException(message: "Mailbox password is required.");
        }
    }

}