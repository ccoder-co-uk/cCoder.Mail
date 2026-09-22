// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using cCoder.Mail.Providers.Models;

namespace cCoder.Mail.Providers.Brokers.MailClients;

internal sealed class MailMessageParsingBroker : IMailMessageParsingBroker
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

    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public EncodedMailWord[] SelectEncodedMailWords(string value) =>
        encodedWordRegex
            .Matches(input: value)
            .Select(selector: match =>
                new EncodedMailWord
                {
                    Value = match.Value,
                    Encoding = match.Groups["encoding"].Value,
                    Text = match.Groups["text"].Value,
                })
            .ToArray();

    public string DecodeQuotedPrintable(string content) =>
        quotedPrintableRegex.Replace(
            input: content,
            evaluator: match =>
                ((char)Convert.ToByte(
                    value: match.Groups[1].Value,
                    fromBase: 16))
                .ToString());

    public string DecodeBase64(string content) =>
        Encoding.UTF8.GetString(
            bytes: Convert.FromBase64String(s: content));

    public string SelectMultipartBoundary(string contentType) =>
        boundaryRegex
            .Match(input: contentType)
            .Groups["boundary"]
            .Value;

    public MicrosoftGraphMessageEnvelope DeserializeMicrosoftGraphMessages(
        string content) =>
        JsonSerializer.Deserialize<MicrosoftGraphMessageEnvelope>(
            json: content,
            options: serializerOptions);
}