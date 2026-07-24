// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models.Exceptions;

public sealed class MailDependencyException(Exception innerException)
    : Exception(
        message: "A mail dependency failed.",
        innerException: innerException);