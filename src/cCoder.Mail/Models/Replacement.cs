// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models;

public class Replacement
{
    private readonly string newString;

    public string Old { get; }
    public string New =>
        newString ?? ReplaceFunction(arg: Old);

    public Func<string, string> ReplaceFunction { get; }

    public Replacement(string old, string @new)
    {
        Old = old;
        newString = @new;
        ReplaceFunction = source => source;
    }

    public Replacement(string old, Func<string, string> replacer)
    {
        Old = old;
        ReplaceFunction = source => source;

        if (replacer is not null)
        {
            ReplaceFunction = replacer;
        }
    }
}