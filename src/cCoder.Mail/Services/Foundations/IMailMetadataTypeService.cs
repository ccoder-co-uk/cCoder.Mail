using cCoder.Mail.Api.OData;


namespace cCoder.Mail.Services.Foundations;

internal interface IMailMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}

