// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Mail.Models.OData;

public class ExtendedMetadataContainer : MetadataContainer
{
    public ExtendedMetadataContainer()
    { }

    public ExtendedMetadataContainer(
        Type type,
        bool isEntity = false,
        bool hasEndpoint = false)
        : base(type: type, isEntity: isEntity, hasEndpoint: hasEndpoint)
    { }

    public IEnumerable<OperationContainer> Operations { get; set; }
}