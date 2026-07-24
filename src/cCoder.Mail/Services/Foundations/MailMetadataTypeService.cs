// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Dependencies.OData;
using cCoder.Data.Models.Mail;


namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailMetadataTypeService : IMailMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
        TryCatch<IEnumerable<MetadataContainerSet>>(operation: () =>
    {

        ValidateGetKnownMetadata(inputs: []);

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
        new(type: typeof(T), isEntity: true, hasEndpoint: true)
        {
            Category = "Mail",
        };
}