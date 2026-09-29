// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Models;

public class Replacement
{
    public string Old { get; set; }
    public string New { get; set; }
    public Func<string, string> ReplaceFunction { get; set; }
}