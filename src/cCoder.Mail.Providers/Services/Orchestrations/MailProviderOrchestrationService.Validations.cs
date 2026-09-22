// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Providers.Services.Orchestrations;

internal sealed partial class MailProviderOrchestrationService
{
    private static void ValidateMailClientOnGet(
        object[] inputs) =>
        Validate(
            inputs: inputs
                .Where(predicate: input => input is not null)
                .ToArray());

    private static void Validate(params object[] inputs)
    {
        foreach (object input in inputs)
        {
            ArgumentNullException.ThrowIfNull(argument: input);
        }
    }
}