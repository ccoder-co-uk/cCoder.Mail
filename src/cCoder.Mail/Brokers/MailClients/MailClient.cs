using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Models;

namespace cCoder.Mail.Brokers.MailClients;

internal sealed partial class MailClient : IMailClient
{
    public async Task SendAsync(QueuedEmail email, CancellationToken cancellationToken = default)
    {
        MailServer server = email.App?.MailServers?.FirstOrDefault(
            mailServer => mailServer.Name == email.MailServerName);

        if (server == null)
            throw new InvalidOperationException("No mail server configuration could be found to send the email.");

        using SmtpClient client = new()
        {
            Host = server.Host,
            Port = server.Port,
            EnableSsl = server.EnableSSL,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(server.User, server.Password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        using MailMessage message = new()
        {
            IsBodyHtml = email.IsBodyHtml,
            Subject = email.Subject,
            Body = email.Content
        };

        if (!string.IsNullOrWhiteSpace(server.FromEmail))
            message.From = new MailAddress(server.FromEmail);

        message.From ??= server.User.Contains('@')
            ? new MailAddress(server.User)
            : null;

        message.To.Add(email.To);
        await client.SendMailAsync(message, cancellationToken);
    }

    public async Task<ReceivedEmail[]> ReceiveAsync(
        MailboxReceiveRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateReceiveRequest(request);

        using TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(request.Host, request.Port, cancellationToken);

        using Stream networkStream = tcpClient.GetStream();
        using Stream stream = request.EnableSSL
            ? await CreateSslStreamAsync(networkStream, request.Host, cancellationToken)
            : networkStream;

        using StreamReader reader = new(stream, Encoding.ASCII, leaveOpen: true);
        using StreamWriter writer = new(stream, Encoding.ASCII, leaveOpen: true)
        {
            NewLine = "\r\n",
            AutoFlush = true,
        };

        await ExpectOkAsync(reader, cancellationToken);
        await SendCommandAsync(writer, reader, $"USER {request.User}", cancellationToken);
        await SendCommandAsync(writer, reader, $"PASS {request.Password}", cancellationToken);

        string stat = await SendCommandAsync(writer, reader, "STAT", cancellationToken);
        int count = ParseMessageCount(stat);
        int maximumMessages = request.MaximumMessages <= 0
            ? count
            : Math.Min(request.MaximumMessages, count);

        List<ReceivedEmail> messages = [];

        for (int index = count; index > 0 && messages.Count < maximumMessages; index--)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string[] rawMessage = await RetrieveMessageAsync(writer, reader, index, cancellationToken);
            ReceivedEmail receivedEmail = ParseMessage(rawMessage);

            if (IsWithinPeriod(receivedEmail.ReceivedOn, request.From, request.To))
                messages.Add(receivedEmail);
        }

        await writer.WriteLineAsync("QUIT");
        return [.. messages.OrderByDescending(message => message.ReceivedOn)];
    }

    public Task<ReceivedEmail[]> ReceiveTopAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        ReceiveAsync(
            new MailboxReceiveRequest
            {
                Host = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_HOST"),
                Port = int.TryParse(ReadEnvironment("CCODER_MAIL_RECEIVE_PORT"), out int port) ? port : 995,
                EnableSSL = !bool.TryParse(ReadEnvironment("CCODER_MAIL_RECEIVE_SSL"), out bool enableSsl) || enableSsl,
                User = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_USER"),
                Password = ReadRequiredEnvironment("CCODER_MAIL_RECEIVE_PASSWORD"),
                MaximumMessages = count,
            },
            cancellationToken);

    private static async Task<Stream> CreateSslStreamAsync(
        Stream networkStream,
        string host,
        CancellationToken cancellationToken)
    {
        SslStream sslStream = new(networkStream, leaveInnerStreamOpen: false);
        await sslStream.AuthenticateAsClientAsync(host, null, enabledSslProtocols: default, checkCertificateRevocation: true);
        cancellationToken.ThrowIfCancellationRequested();
        return sslStream;
    }

    private static async Task<string> SendCommandAsync(
        StreamWriter writer,
        StreamReader reader,
        string command,
        CancellationToken cancellationToken)
    {
        await writer.WriteLineAsync(command);
        return await ExpectOkAsync(reader, cancellationToken);
    }

    private static async Task<string> ExpectOkAsync(StreamReader reader, CancellationToken cancellationToken)
    {
        string line = await reader.ReadLineAsync(cancellationToken)
            ?? throw new InvalidOperationException("The mail server closed the connection.");

        if (!line.StartsWith("+OK", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(line);

        return line;
    }

    private static async Task<string[]> RetrieveMessageAsync(
        StreamWriter writer,
        StreamReader reader,
        int index,
        CancellationToken cancellationToken)
    {
        await SendCommandAsync(writer, reader, $"RETR {index}", cancellationToken);

        List<string> lines = [];

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
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

    private static DateTimeOffset? ParseDate(string value) =>
        DateTimeOffset.TryParse(value, out DateTimeOffset parsed)
            ? parsed
            : null;

    private static bool IsWithinPeriod(DateTimeOffset? receivedOn, DateTimeOffset? from, DateTimeOffset? to)
    {
        if (receivedOn is null)
            return from is null && to is null;

        return (from is null || receivedOn >= from)
            && (to is null || receivedOn <= to);
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

    private static string ReadRequiredEnvironment(string variableName) =>
        ReadEnvironment(variableName)
        ?? throw new InvalidOperationException($"{variableName} is required to receive mailbox messages.");

    private static string ReadEnvironment(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variableName)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private readonly record struct ParsedBody(string Content, bool IsBodyHtml);

    [GeneratedRegex("boundary=\"?(?<boundary>[^\";]+)\"?", RegexOptions.IgnoreCase)]
    private static partial Regex BoundaryRegex();

    [GeneratedRegex("=([0-9A-F]{2})", RegexOptions.IgnoreCase)]
    private static partial Regex QuotedPrintableRegex();

    [GeneratedRegex(@"=\?(?<charset>[^?]+)\?(?<encoding>[BQ])\?(?<text>[^?]+)\?=", RegexOptions.IgnoreCase)]
    private static partial Regex EncodedWordRegex();
}
