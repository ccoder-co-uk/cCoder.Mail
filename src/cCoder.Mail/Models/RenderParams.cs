// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Models;

public abstract class RenderParams
{
    public App App { get; set; }
    public string Culture { get; set; }
    public User User { get; set; }
}