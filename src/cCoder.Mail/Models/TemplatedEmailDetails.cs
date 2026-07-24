// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models;

public class TemplatedEmailDetails
{
    public string SourceDomain { get; set; }
    public string TemplateName { get; set; }
    public string Subject { get; set; }
    public string Culture { get; set; }
    public object Model { get; set; }
    public string ToEmail { get; set; }
}