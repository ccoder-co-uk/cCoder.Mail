// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Mail.Exposures.Setup;

public static partial class UIBaseline
{
    public static Package[] Packages =>
        [
        Components,
        Pages,
        PageRoles
    ];
}