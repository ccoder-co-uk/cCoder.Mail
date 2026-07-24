// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models.Exceptions;

public sealed class MailValidationException(Exception innerException)
    : Exception(
        message: "Mail validation failed.",
        innerException: innerException);