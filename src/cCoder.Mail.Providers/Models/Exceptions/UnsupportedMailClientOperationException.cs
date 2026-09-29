// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Providers.Models.Exceptions;

public sealed class UnsupportedMailClientOperationException(
    string providerName,
    string operation)
    : NotSupportedException(
        message:
            $"The '{providerName}' mail provider does not support the '{operation}' operation.")
{
}