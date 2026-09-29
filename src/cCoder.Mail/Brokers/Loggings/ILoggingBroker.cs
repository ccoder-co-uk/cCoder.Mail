// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Brokers.Loggings;

public interface ILoggingBroker
{
    void LogInformation(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}