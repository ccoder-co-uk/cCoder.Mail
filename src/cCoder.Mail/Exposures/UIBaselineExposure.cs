// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Exposures.Setup;

namespace cCoder.Mail.Exposures;

internal sealed class UIBaselineExposure : IUIBaselineExposure
{
    public object GetPackages() =>
        UIBaseline.Packages;
}