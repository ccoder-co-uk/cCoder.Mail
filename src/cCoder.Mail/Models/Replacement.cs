// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models;

public class Replacement
{
    public string Old { get; set; }
    public string New { get; set; }
    public Func<string, string> ReplaceFunction { get; set; }
}