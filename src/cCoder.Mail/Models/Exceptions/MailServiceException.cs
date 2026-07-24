// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models.Exceptions;

public sealed class MailServiceException(Exception innerException)
    : Exception(
        message: "The mail service failed.",
        innerException: innerException);