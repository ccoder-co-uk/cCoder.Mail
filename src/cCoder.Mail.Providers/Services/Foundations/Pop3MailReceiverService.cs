// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Mail.Providers.Brokers.MailClients;
using cCoder.Mail.Providers.Brokers.Storages;
using cCoder.Mail.Providers.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class Pop3MailReceiverService(
    IPop3MailReceiverBroker pop3MailReceiverBroker,
    IMailReceiverStorageBroker mailReceiverStorageBroker)
    : IPop3MailReceiverService
{
    private static readonly Regex boundaryRegex = new(
        pattern: "boundary=\"?(?<boundary>[^\";]+)\"?",
        options: RegexOptions.IgnoreCase);

    private static readonly Regex quotedPrintableRegex = new(
        pattern: "=([0-9A-F]{2})",
        options: RegexOptions.IgnoreCase);

    private static readonly Regex encodedWordRegex = new(
        pattern: @"=\?(?<charset>[^?]+)\?(?<encoding>[BQ])\?(?<text>[^?]+)\?=",
        options: RegexOptions.IgnoreCase);

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

            MailboxReceiveRequest request =
                CreateMailboxReceiveRequest(
                    mailReceiver: mailReceiver,
                    maximumMessages: maximumMessages);

            string[][] rawMessages =
                await ReceiveRawMessagesAsync(
                    request: request,
                    cancellationToken: cancellationToken);

            return [
                .. rawMessages
                .Select(selector: ParseMessage)
                .OrderByDescending(keySelector: message => message.ReceivedOn)
            ];
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

        return pop3MailReceiverBroker.ReceiveAsync(
            request: request,
            cancellationToken: cancellationToken);
    }

    private static ReceivedEmail ParseMessage(string[] lines)
    {
        int separatorIndex = Array.FindIndex(array: lines, match: string.IsNullOrWhiteSpace);
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

    private static ParsedBody ParseBody(
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

    private static ParsedBody ParseMultipartBody(string[] bodyLines, string contentType)
    {
        string boundary = boundaryRegex
            .Match(input: contentType ?? string.Empty)
            .Groups["boundary"].Value;

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

            string[] lines = normalized.Split(separator: '\n')
                .Select(selector: line => line.TrimEnd(trimChar: '\r'))
                .ToArray();

            int separatorIndex = Array.FindIndex(array: lines, match: string.IsNullOrWhiteSpace);

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

    private static string DecodeBody(string content, string transferEncoding) =>
        transferEncoding?.Equals(value: "base64", comparisonType: StringComparison.OrdinalIgnoreCase) == true
            ? DecodeBase64(content: content)
            : transferEncoding?.Equals(value: "quoted-printable", comparisonType: StringComparison.OrdinalIgnoreCase) == true
                ? DecodeQuotedPrintable(content: content)
                : content;

    private static string DecodeBase64(string content)
    {
        try
        {
            return Encoding.UTF8.GetString(bytes: Convert.FromBase64String(s: RemoveWhitespace(value: content)));
        }
        catch (FormatException)
        {
            return content;
        }
    }

    private static string DecodeQuotedPrintable(string content)
    {
        string unfolded = content.Replace(oldValue: "=\r\n", newValue: string.Empty)
            .Replace(oldValue: "=\n", newValue: string.Empty);

        return quotedPrintableRegex
            .Replace(
input: unfolded,
evaluator: match => ((char)Convert.ToByte(value: match.Groups[1].Value, fromBase: 16)).ToString());
    }

    private static string DecodeHeader(string value)
    {
        if (string.IsNullOrWhiteSpace(value: value))
        {
            return value;
        }

        return encodedWordRegex
            .Replace(input: value, evaluator: match =>
        {
            string encoding = match.Groups["encoding"].Value;
            string encodedText = match.Groups["text"].Value;

            return string.Equals(a: encoding, b: "B", comparisonType: StringComparison.OrdinalIgnoreCase)
                ? DecodeBase64(content: encodedText)
                : DecodeQuotedPrintable(content: encodedText.Replace(oldChar: '_', newChar: ' '));
        });
    }

    private static DateTimeOffset ParseDate(string value) =>
        DateTimeOffset.TryParse(input: value, result: out DateTimeOffset parsed)
            ? parsed
            : DateTimeOffset.MinValue;

    private static string Header(Dictionary<string, string> headers, string name) =>
        headers.TryGetValue(key: name, value: out string value) ? value : null;

    private static string RemoveWhitespace(string value) =>
        string.Concat(values: (value ?? string.Empty).Where(predicate: character => !char.IsWhiteSpace(c: character)));

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

    private readonly record struct ParsedBody(string Content, bool IsBodyHtml);

}