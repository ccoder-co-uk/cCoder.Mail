// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models.OData;

public class OperationContainer
{
    public string Definition { get; set; }
    public string HttpVerb { get; set; }
    public string Name { get; set; }
    public IDictionary<string, string> Parameters { get; set; }
    public bool Queryable { get; set; }
    public MetadataContainer ReturnType { get; set; }
    public string Url { get; set; }
}