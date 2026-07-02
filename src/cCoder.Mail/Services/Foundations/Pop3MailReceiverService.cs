using cCoder.Data.Models.Mail;
using cCoder.Mail.Brokers.MailClients;
using cCoder.Mail.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class Pop3MailReceiverService(
    IPop3MailReceiverBroker pop3MailReceiverBroker,
    MailConfiguration mailConfiguration)
    : IPop3MailReceiverService
{
    public async Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default)
    {
        string[][] rawMessages = await ReceiveRawMessagesAsync(request, cancellationToken);

        return
        [
            .. rawMessages
                .Select(ParseMessage)
                .Where(message => IsWithinPeriod(message.ReceivedOn, request.From, request.To))
                .OrderByDescending(message => message.ReceivedOn)
        ];
    }

    public async Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        string[][] rawMessages = await ReceiveRawMessagesAsync(
            new MailboxReceiveRequest
            {
                ProviderName = MailProviderNames.Pop3,
                Host = ReadRequiredConfiguration(mailConfiguration.Pop3.Host, "POP3 mailbox host"),
                Port = mailConfiguration.Pop3.Port,
                EnableSSL = mailConfiguration.Pop3.EnableSSL,
                User = ReadRequiredConfiguration(mailConfiguration.Pop3.User, "POP3 mailbox user"),
                Password = ReadRequiredConfiguration(mailConfiguration.Pop3.Password, "POP3 mailbox password"),
                MaximumMessages = count,
            },
            cancellationToken);

        return [.. rawMessages.Select(ParseMessage).OrderByDescending(message => message.ReceivedOn)];
    }

    private async Task<string[][]> ReceiveRawMessagesAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken)
    {
        ValidateReceiveRequest(request);

        using MailClientTextConnection connection = await OpenConnectionAsync(request, cancellationToken);
        await ExpectOkAsync(connection, cancellationToken);
        await SendCommandAsync(connection, $"USER {request.User}", cancellationToken);
        await SendCommandAsync(connection, $"PASS {request.Password}", cancellationToken);

        string stat = await SendCommandAsync(connection, "STAT", cancellationToken);
        int count = ParseMessageCount(stat);
        int maximumMessages = request.MaximumMessages <= 0
            ? count
            : Math.Min(request.MaximumMessages, count);

        List<string[]> messages = [];

        for (int index = count; index > 0 && messages.Count < maximumMessages; index--)
        {
            cancellationToken.ThrowIfCancellationRequested();
            messages.Add(await RetrieveMessageAsync(connection, index, cancellationToken));
        }

        await pop3MailReceiverBroker.WriteLineAsync(connection, "QUIT", cancellationToken);
        return [.. messages];
    }

    private Task<MailClientTextConnection> OpenConnectionAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken) =>
        request.EnableSSL
            ? pop3MailReceiverBroker.OpenSslAsync(request.Host, request.Port, cancellationToken)
            : pop3MailReceiverBroker.OpenAsync(request.Host, request.Port, cancellationToken);

    private async Task<string> SendCommandAsync(
        MailClientTextConnection connection,
        string command,
        CancellationToken cancellationToken)
    {
        await pop3MailReceiverBroker.WriteLineAsync(connection, command, cancellationToken);
        return await ExpectOkAsync(connection, cancellationToken);
    }

    private async Task<string> ExpectOkAsync(
        MailClientTextConnection connection,
        CancellationToken cancellationToken)
    {
        string line = await pop3MailReceiverBroker.ReadLineAsync(connection, cancellationToken)
            ?? throw new InvalidOperationException("The mail server closed the connection.");

        if (!line.StartsWith("+OK", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(line);

        return line;
    }

    private async Task<string[]> RetrieveMessageAsync(
        MailClientTextConnection connection,
        int index,
        CancellationToken cancellationToken)
    {
        await SendCommandAsync(connection, $"RETR {index}", cancellationToken);

        List<string> lines = [];

        while (await pop3MailReceiverBroker.ReadLineAsync(connection, cancellationToken) is { } line)
        {
            if (line == ".")
                break;

            lines.Add(line.StartsWith("..", StringComparison.Ordinal) ? line[1..] : line);
        }

        return [.. lines];
    }

    private static int ParseMessageCount(string stat)
    {
        string[] parts = stat.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 && int.TryParse(parts[1], out int count) ? count : 0;
    }

    private static ReceivedEmail ParseMessage(string[] lines)
    {
        int separatorIndex = Array.FindIndex(lines, string.IsNullOrWhiteSpace);
        string[] headerLines = separatorIndex >= 0 ? lines[..separatorIndex] : lines;
        string[] bodyLines = separatorIndex >= 0 ? lines[(separatorIndex + 1)..] : [];
        Dictionary<string, string> headers = ParseHeaders(headerLines);
        string contentType = Header(headers, "Content-Type");
        string transferEncoding = Header(headers, "Content-Transfer-Encoding");
        ParsedBody body = ParseBody(bodyLines, contentType, transferEncoding);

        return new ReceivedEmail
        {
            MessageId = Header(headers, "Message-ID"),
            From = Header(headers, "From"),
            To = Header(headers, "To"),
            CC = Header(headers, "Cc"),
            Subject = DecodeHeader(Header(headers, "Subject")),
            Content = body.Content,
            IsBodyHtml = body.IsBodyHtml,
            ReceivedOn = ParseDate(Header(headers, "Date")),
        };
    }

    private static Dictionary<string, string> ParseHeaders(string[] lines)
    {
        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase);
        string currentName = null;

        foreach (string line in lines)
        {
            if ((line.StartsWith(' ') || line.StartsWith('\t')) && currentName != null)
            {
                headers[currentName] += " " + line.Trim();
                continue;
            }

            int separatorIndex = line.IndexOf(':');

            if (separatorIndex <= 0)
                continue;

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
        if (contentType?.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase) == true)
            return ParseMultipartBody(bodyLines, contentType);

        string content = DecodeBody(string.Join("\n", bodyLines), transferEncoding);

        return new ParsedBody(
            content,
            contentType?.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) == true);
    }

    private static ParsedBody ParseMultipartBody(string[] bodyLines, string contentType)
    {
        string boundary = BoundaryRegex().Match(contentType ?? string.Empty).Groups["boundary"].Value;

        if (string.IsNullOrWhiteSpace(boundary))
            return new ParsedBody(string.Join("\n", bodyLines), false);

        string rawBody = string.Join("\n", bodyLines);
        string[] sections = rawBody.Split($"--{boundary}", StringSplitOptions.RemoveEmptyEntries);
        ParsedBody fallback = new(string.Empty, false);

        foreach (string section in sections)
        {
            string normalized = section.Trim('\r', '\n', '-');

            if (string.IsNullOrWhiteSpace(normalized))
                continue;

            string[] lines = normalized.Split('\n').Select(line => line.TrimEnd('\r')).ToArray();
            int separatorIndex = Array.FindIndex(lines, string.IsNullOrWhiteSpace);

            if (separatorIndex < 0)
                continue;

            Dictionary<string, string> headers = ParseHeaders(lines[..separatorIndex]);
            string partContentType = Header(headers, "Content-Type");
            string partTransferEncoding = Header(headers, "Content-Transfer-Encoding");
            string content = DecodeBody(
                string.Join("\n", lines[(separatorIndex + 1)..]),
                partTransferEncoding);

            if (partContentType?.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) == true)
                return new ParsedBody(content, true);

            if (partContentType?.StartsWith("text/plain", StringComparison.OrdinalIgnoreCase) == true)
                fallback = new ParsedBody(content, false);
        }

        return fallback;
    }

    private static string DecodeBody(string content, string transferEncoding) =>
        transferEncoding?.Equals("base64", StringComparison.OrdinalIgnoreCase) == true
            ? DecodeBase64(content)
            : transferEncoding?.Equals("quoted-printable", StringComparison.OrdinalIgnoreCase) == true
                ? DecodeQuotedPrintable(content)
                : content;

    private static string DecodeBase64(string content)
    {
        try
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(RemoveWhitespace(content)));
        }
        catch (FormatException)
        {
            return content;
        }
    }

    private static string DecodeQuotedPrintable(string content)
    {
        string unfolded = content.Replace("=\r\n", string.Empty).Replace("=\n", string.Empty);
        return QuotedPrintableRegex().Replace(
            unfolded,
            match => ((char)Convert.ToByte(match.Groups[1].Value, 16)).ToString());
    }

    private static string DecodeHeader(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return EncodedWordRegex().Replace(value, match =>
        {
            string encoding = match.Groups["encoding"].Value;
            string encodedText = match.Groups["text"].Value;

            return string.Equals(encoding, "B", StringComparison.OrdinalIgnoreCase)
                ? DecodeBase64(encodedText)
                : DecodeQuotedPrintable(encodedText.Replace('_', ' '));
        });
    }

    private static DateTimeOffset ParseDate(string value) =>
        DateTimeOffset.TryParse(value, out DateTimeOffset parsed)
            ? parsed
            : DateTimeOffset.MinValue;

    private static bool IsWithinPeriod(DateTimeOffset receivedOn, DateTimeOffset? from, DateTimeOffset? to)
    {
        if (receivedOn == DateTimeOffset.MinValue)
            return from is null && to is null;

        return (from is null || receivedOn >= from.Value)
            && (to is null || receivedOn <= to.Value);
    }

    private static string Header(Dictionary<string, string> headers, string name) =>
        headers.TryGetValue(name, out string value) ? value : null;

    private static string RemoveWhitespace(string value) =>
        string.Concat((value ?? string.Empty).Where(character => !char.IsWhiteSpace(character)));

    private static void ValidateReceiveRequest(MailboxReceiveRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Host))
            throw new InvalidOperationException("Mailbox host is required.");

        if (request.Port <= 0)
            throw new InvalidOperationException("Mailbox port is required.");

        if (string.IsNullOrWhiteSpace(request.User))
            throw new InvalidOperationException("Mailbox user is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Mailbox password is required.");
    }

    private static string ReadRequiredConfiguration(string value, string configurationName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new InvalidOperationException($"{configurationName} is required to receive mailbox messages.")
            : value;

    private readonly record struct ParsedBody(string Content, bool IsBodyHtml);

    [GeneratedRegex("boundary=\"?(?<boundary>[^\";]+)\"?", RegexOptions.IgnoreCase)]
    private static partial Regex BoundaryRegex();

    [GeneratedRegex("=([0-9A-F]{2})", RegexOptions.IgnoreCase)]
    private static partial Regex QuotedPrintableRegex();

    [GeneratedRegex(@"=\?(?<charset>[^?]+)\?(?<encoding>[BQ])\?(?<text>[^?]+)\?=", RegexOptions.IgnoreCase)]
    private static partial Regex EncodedWordRegex();
}
