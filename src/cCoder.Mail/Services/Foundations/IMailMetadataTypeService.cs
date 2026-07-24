// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Exposures.OData;


namespace cCoder.Mail.Services.Foundations;

internal interface IMailMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}