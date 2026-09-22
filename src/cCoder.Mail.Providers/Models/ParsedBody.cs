// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Models;

internal readonly record struct ParsedBody(
    string Content,
    bool IsBodyHtml);