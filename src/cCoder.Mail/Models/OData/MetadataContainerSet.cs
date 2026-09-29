// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
namespace cCoder.Mail.Models.OData;

public class MetadataContainerSet
{
    public string Name { get; set; }
    public ExtendedMetadataContainer[] Types { get; set; }
    public string UriBase { get; set; }
}