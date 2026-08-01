// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Extensions.OData;

namespace cCoder.Mail.Models.OData;

public class MetadataContainer
{
    public MetadataContainer()
    { }

    public MetadataContainer(Type type)
    {
        IsValueType = type.IsValueType || type == typeof(string);
        Type = type.GetMetadataTypeName();
        Name = type.Name;
        DisplayName = type.Name;
        Description = type.Name;
        ServerType = type.AssemblyQualifiedName;
        ServerTypeName = type.GetCSharpTypeName();
        Properties = type.IsValueType || type == typeof(string)
            ? []
            : type.GetProperties()
                .Select(
                    selector: property =>
                        property.CreatePropertyContainer())
                .ToArray();
    }

    public MetadataContainer(Type type, bool isEntity, bool hasEndpoint)
        : this(type: type)
    {
        IsEntity = isEntity;
        IsJoinEntity = isEntity && type.IsJoinType();
        HasEndpoint = hasEndpoint;
    }

    public string Category { get; set; }
    public string Description { get; set; }
    public string DisplayName { get; set; }
    public bool HasEndpoint { get; set; }
    public bool IsEntity { get; set; }
    public bool IsJoinEntity { get; set; }
    public bool IsSystemManaged { get; set; }
    public bool IsValueType { get; set; }
    public string Name { get; set; }
    public IEnumerable<PropertyContainer> Properties { get; set; }
    public string ServerType { get; set; }
    public string ServerTypeName { get; set; }
    public string Type { get; set; }
}