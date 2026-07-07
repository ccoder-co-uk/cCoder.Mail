using cCoder.Data.Models.Packaging;

namespace cCoder.Mail.Exposures.Setup;

public static partial class UIBaseline
{
    static Package PageRoles => new()
    {
        Name = "Mail Page Roles",
        Category = "Mail",
        Description = "Mail Page Roles.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/MailManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/MailManagement",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/MailManagement",
  "Role": "Guests"
}
"""
            }
        ]
    };
}