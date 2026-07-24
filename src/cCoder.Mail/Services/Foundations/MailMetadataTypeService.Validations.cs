// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailMetadataTypeService
{
    private static void ValidateGetKnownMetadata(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}