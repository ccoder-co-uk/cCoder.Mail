// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Models;

public class TemplateRenderParams(App app, User user, string culture)
    : RenderParams(app: app, user: user, culture: culture)
{ }