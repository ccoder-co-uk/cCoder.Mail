// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;


namespace cCoder.Mail.Models.OData;

public class ODataModel
{
    public ODataModel()
    {
        Context = string.Empty;
        Description = string.Empty;
    }

    public string Context { get; set; }

    public string Description { get; set; }

    public IEdmModel EDMModel { get; set; }
}