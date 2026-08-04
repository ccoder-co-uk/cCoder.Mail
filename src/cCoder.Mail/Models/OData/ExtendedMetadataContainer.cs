// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models.OData;

public class ExtendedMetadataContainer : MetadataContainer
{
    public IEnumerable<OperationContainer> Operations { get; set; }
}