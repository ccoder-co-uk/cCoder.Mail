// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies;

namespace cCoder.Mail.Services.Foundations.Events;

internal partial class EventHandlerService
{
    private static void ValidateListenToAllEvents(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}