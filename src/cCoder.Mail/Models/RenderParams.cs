// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Models;

public abstract class RenderParams
{
    protected RenderParams(App app, User user)
        : this(app: app, user: user, culture: "")
    { }

    protected RenderParams(App app, User user, string culture)
    {
        App = app;
        User = user;
        Culture = culture;
    }

    public App App { get; }
    public string Culture { get; set; }
    public User User { get; }
}