// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Providers.Models.Exceptions;

public sealed class MailServiceException(Exception innerException)
    : Exception(
        message: "The mail service failed.",
        innerException: innerException);