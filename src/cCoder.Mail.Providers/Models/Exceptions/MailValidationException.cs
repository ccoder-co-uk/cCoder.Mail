// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Providers.Models.Exceptions;

public sealed class MailValidationException(Exception innerException)
    : Exception(
        message: "Mail validation failed.",
        innerException: innerException);