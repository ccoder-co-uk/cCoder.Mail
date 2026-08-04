// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.OData;
using cCoder.Mail.Models.OData;
using cCoder.Data.Models.Mail;
using cCoder.Mail.Extensions.OData;


namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailMetadataTypeService : IMailMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
        TryCatch<IEnumerable<MetadataContainerSet>>(operation: () =>
    {

        ValidateKnownMetadataOnGet(inputs: []);

        return [
                new MetadataContainerSet
        {
            Name = "Mail",
            UriBase = "Mail",
            Types =
            [
                Entity<MailServer>(),
                Entity<QueuedEmail>(),
                Entity<SentEmail>(),
            ],
        },
    ];
    });

    private static ExtendedMetadataContainer Entity<T>() =>
        CreateExtendedMetadataContainer<T>();

    private static ExtendedMetadataContainer CreateExtendedMetadataContainer<T>()
    {
        ExtendedMetadataContainer metadata = typeof(T).CreateExtendedMetadataContainer(
            isEntity: true,
            hasEndpoint: true);

        metadata.Category = "Mail";

        return metadata;
    }
}