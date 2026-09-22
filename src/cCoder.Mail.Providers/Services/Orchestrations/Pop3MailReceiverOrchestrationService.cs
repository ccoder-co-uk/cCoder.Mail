// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Models;
using cCoder.Mail.Providers.Services.Foundations;
using System.Text;

namespace cCoder.Mail.Providers.Services.Orchestrations;

internal sealed partial class Pop3MailReceiverOrchestrationService(
    IPop3MailboxService pop3MailboxService,
    IMailReceiverProviderService mailReceiverProviderService,
    IMailMessageParsingService mailMessageParsingService)
    : IPop3MailReceiverOrchestrationService
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
                await mailReceiverProviderService
                    .RetrieveMailReceiverAsync(
                        mailReceiverId: mailReceiverId,
                        cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException(
                    message:
                        $"Mail receiver '{mailReceiverId}' was not found.");

            MailboxReceiveRequest request =
                CreateMailboxReceiveRequest(
                    mailReceiver: mailReceiver,
                    maximumMessages: maximumMessages);

            string[][] rawMessages =
                await ReceiveRawMessagesAsync(
                    request: request,
                    cancellationToken: cancellationToken);

            List<ReceivedEmail> messages = [];

            foreach (string[] rawMessage in rawMessages)
            {
                messages.Add(item: ParseMessage(lines: rawMessage));
            }

            messages.Sort(comparison: (left, right) =>
                right.ReceivedOn.CompareTo(other: left.ReceivedOn));

            return messages.ToArray();
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

    private Task<string[][]> ReceiveRawMessagesAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken)
    {
        ValidateReceiveRequest(request: request);

        return pop3MailboxService.RetrieveMailboxReceiveRequestMessagesAsync(
            mailboxReceiveRequest: request,
            cancellationToken: cancellationToken);
    }

    private ReceivedEmail ParseMessage(string[] lines)
    {
        int separatorIndex = FindSeparatorIndex(lines: lines);
        string[] headerLines = separatorIndex >= 0 ? lines[..separatorIndex] : lines;
        string[] bodyLines = separatorIndex >= 0 ? lines[(separatorIndex + 1)..] : [];
        Dictionary<string, string> headers = ParseHeaders(lines: headerLines);
        string contentType = Header(headers: headers, name: "Content-Type");
        string transferEncoding = Header(headers: headers, name: "Content-Transfer-Encoding");
        ParsedBody body = ParseBody(bodyLines: bodyLines, contentType: contentType, transferEncoding: transferEncoding);

        return new ReceivedEmail
        {
            MessageId = Header(headers: headers, name: "Message-ID"),
            From = Header(headers: headers, name: "From"),
            To = Header(headers: headers, name: "To"),
            CC = Header(headers: headers, name: "Cc"),
            Subject = DecodeHeader(value: Header(headers: headers, name: "Subject")),
            Content = body.Content,
            IsBodyHtml = body.IsBodyHtml,
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

    private ParsedBody ParseBody(
        string[] bodyLines,
        string contentType,
        string transferEncoding)
    {
        if (contentType?.StartsWith(value: "multipart/", comparisonType: StringComparison.OrdinalIgnoreCase) == true)
        {
            return ParseMultipartBody(bodyLines: bodyLines, contentType: contentType);
        }

        string content = DecodeBody(content: string.Join(separator: "\n", value: bodyLines), transferEncoding: transferEncoding);

        return new ParsedBody(
Content: content,
IsBodyHtml: contentType?.StartsWith(value: "text/html", comparisonType: StringComparison.OrdinalIgnoreCase) == true);
    }

    private ParsedBody ParseMultipartBody(string[] bodyLines, string contentType)
    {
        string boundary = mailMessageParsingService
            .RetrieveMultipartBoundary(
                contentType: contentType ?? string.Empty);

        if (string.IsNullOrWhiteSpace(value: boundary))
        {
            return new ParsedBody(Content: string.Join(separator: "\n", value: bodyLines), IsBodyHtml: false);
        }

        string rawBody = string.Join(separator: "\n", value: bodyLines);
        string[] sections = rawBody.Split(separator: $"--{boundary}", options: StringSplitOptions.RemoveEmptyEntries);
        ParsedBody fallback = new(Content: string.Empty, IsBodyHtml: false);

        foreach (string section in sections)
        {
            string normalized = section.Trim(
                trimChars: ['\r', '\n', '-']);

            if (string.IsNullOrWhiteSpace(value: normalized))
            {
                continue;
            }

            string[] lines = normalized.Split(separator: '\n');

            for (int index = 0; index < lines.Length; index++)
            {
                lines[index] = lines[index].TrimEnd(trimChar: '\r');
            }

            int separatorIndex = FindSeparatorIndex(lines: lines);

            if (separatorIndex < 0)
            {
                continue;
            }

            Dictionary<string, string> headers = ParseHeaders(lines: lines[..separatorIndex]);
            string partContentType = Header(headers: headers, name: "Content-Type");
            string partTransferEncoding = Header(headers: headers, name: "Content-Transfer-Encoding");

            string content = DecodeBody(
content: string.Join(separator: "\n", value: lines[(separatorIndex + 1)..]),
transferEncoding: partTransferEncoding);

            if (partContentType?.StartsWith(value: "text/html", comparisonType: StringComparison.OrdinalIgnoreCase) == true)
            {
                return new ParsedBody(Content: content, IsBodyHtml: true);
            }

            if (partContentType?.StartsWith(value: "text/plain", comparisonType: StringComparison.OrdinalIgnoreCase) == true)
            {
                fallback = new ParsedBody(Content: content, IsBodyHtml: false);
            }
        }

        return fallback;
    }

    private string DecodeBody(string content, string transferEncoding) =>
        transferEncoding?.Equals(value: "base64", comparisonType: StringComparison.OrdinalIgnoreCase) == true
            ? DecodeBase64(content: content)
            : transferEncoding?.Equals(value: "quoted-printable", comparisonType: StringComparison.OrdinalIgnoreCase) == true
                ? DecodeQuotedPrintable(content: content)
                : content;

    private string DecodeBase64(string content) =>
        mailMessageParsingService.DecodeBase64(
            content: RemoveWhitespace(value: content));

    private string DecodeQuotedPrintable(string content) =>
        mailMessageParsingService.DecodeQuotedPrintable(
            content: content.Replace(oldValue: "=\r\n", newValue: string.Empty)
                .Replace(oldValue: "=\n", newValue: string.Empty));

    private string DecodeHeader(string value)
    {
        string decoded = value;

        foreach (EncodedMailWord word in mailMessageParsingService
            .RetrieveEncodedMailWords(value: value ?? string.Empty))
        {
            string replacement = string.Equals(
                a: word.Encoding,
                b: "B",
                comparisonType: StringComparison.OrdinalIgnoreCase)
                    ? DecodeBase64(content: word.Text)
                    : DecodeQuotedPrintable(
                        content: word.Text.Replace(oldChar: '_', newChar: ' '));

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

    private static string Header(Dictionary<string, string> headers, string name) =>
        headers.TryGetValue(key: name, value: out string value) ? value : null;

    private static string RemoveWhitespace(string value)
    {
        StringBuilder result = new();

        foreach (char character in value ?? string.Empty)
        {
            if (!char.IsWhiteSpace(c: character))
            {
                result.Append(value: character);
            }
        }

        return result.ToString();
    }

    private static int FindSeparatorIndex(string[] lines)
    {
        for (int index = 0; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(value: lines[index]))
            {
                return index;
            }
        }

        return -1;
    }

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