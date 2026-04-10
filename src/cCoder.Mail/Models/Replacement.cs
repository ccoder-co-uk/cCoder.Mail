namespace cCoder.Mail.Models;

public class Replacement
{
    private readonly string newString;

    public string Old { get; }
    public string New => newString ?? ReplaceFunction(Old);
    public Func<string, string> ReplaceFunction { get; } = source => source;

    public Replacement(string old, string @new)
    {
        Old = old;
        newString = @new;
    }

    public Replacement(string old, Func<string, string> replacer)
    {
        Old = old;

        if (replacer is not null)
            ReplaceFunction = replacer;
    }
}

