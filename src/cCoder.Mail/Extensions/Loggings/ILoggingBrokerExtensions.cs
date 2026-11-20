// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.Loggings;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Extensions.Loggings;

internal static class ILoggingBrokerExtensions
{
    internal static IActionResult LogSuccessful(
        this ILoggingBroker loggingBroker,
        IActionResult result)
    {
        loggingBroker.LogInformation(message: "Controller request succeeded.");

        return result;
    }
}