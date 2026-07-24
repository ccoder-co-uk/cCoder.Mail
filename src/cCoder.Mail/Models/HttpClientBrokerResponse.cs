// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models;

public readonly record struct HttpClientBrokerResponse(bool IsSuccessStatusCode, string Content);