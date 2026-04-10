using cCoder.Mail.Api.OData;
using cCoder.Data.Models.Mail;


namespace cCoder.Mail.Services.Foundations;

internal sealed class MailMetadataTypeService : IMailMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
    [
        new MetadataContainerSet
        {
            Name = "Core",
            UriBase = "Core",
            Types =
            [
                Entity<MailServer>(),
                Entity<QueuedEmail>(),
                Entity<SentEmail>(),
            ],
        },
    ];

    private static ExtendedMetadataContainer Entity<T>() =>
        new(typeof(T), isEntity: true, hasEndpoint: true)
        {
            Category = "Core",
        };
}

