// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Collections;
using cCoder.Mail.Models.OData;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailMetadataTypeService
{
    private MetadataContainer CreateMetadataContainer(
        Type type,
        bool isEntity = false,
        bool hasEndpoint = false)
    {
        bool isValueType = type.IsValueType || type == typeof(string);

        return new MetadataContainer
        {
            IsValueType = isValueType,
            Type = GetMetadataTypeName(type: type),
            Name = type.Name,
            DisplayName = type.Name,
            Description = type.Name,
            ServerType = type.AssemblyQualifiedName,
            ServerTypeName = GetCSharpTypeName(type: type),
            Properties = isValueType
                ? []
                : type.GetProperties()
                    .Select(selector: CreatePropertyContainer)
                    .ToArray(),
            IsEntity = isEntity,
            IsJoinEntity = isEntity && IsJoinType(type: type),
            HasEndpoint = hasEndpoint,
        };
    }

    private PropertyContainer CreatePropertyContainer(
        PropertyInfo property) =>
        new()
        {
            Name = property.Name,
            Type = GetMetadataTypeName(type: property.PropertyType),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = GetCSharpTypeName(type: property.PropertyType),
            IsValueType = property.PropertyType.IsValueType
                || property.PropertyType == typeof(string),
            DisplayName = property.Name,
            ShortDisplayName = property.Name,
            Description = property.Name,
            IsReadOnly = !property.CanWrite,
            Template = attributeBroker.GetCustomAttribute<KeyAttribute>(
                memberInfo: property) is not null
                || property.Name == "Id"
                    ? "key"
                    : property.Name,
            IsRequired = (!(property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition()
                        == typeof(Nullable<>))
                    && property.PropertyType.IsValueType)
                || attributeBroker.GetCustomAttribute<RequiredAttribute>(
                    memberInfo: property)
                    is not null
        };

    private ExtendedMetadataContainer CreateExtendedMetadataContainer(
        Type type,
        bool isEntity = false,
        bool hasEndpoint = false)
    {
        MetadataContainer metadata = CreateMetadataContainer(
            type: type,
            isEntity: isEntity,
            hasEndpoint: hasEndpoint);

        return new ExtendedMetadataContainer
        {
            IsValueType = metadata.IsValueType,
            Type = metadata.Type,
            Name = metadata.Name,
            DisplayName = metadata.DisplayName,
            Description = metadata.Description,
            ServerType = metadata.ServerType,
            ServerTypeName = metadata.ServerTypeName,
            Properties = metadata.Properties,
            IsEntity = metadata.IsEntity,
            IsJoinEntity = metadata.IsJoinEntity,
            HasEndpoint = metadata.HasEndpoint,
        };
    }

    private static readonly Dictionary<Type, string> TypeNames = new()
    {
        { typeof(short), "number" },
        { typeof(int), "number" },
        { typeof(long), "number" },
        { typeof(short?), "number" },
        { typeof(int?), "number" },
        { typeof(long?), "number" },
        { typeof(ushort), "number" },
        { typeof(uint), "number" },
        { typeof(ulong), "number" },
        { typeof(ushort?), "number" },
        { typeof(uint?), "number" },
        { typeof(ulong?), "number" },
        { typeof(byte), "number" },
        { typeof(byte?), "number" },
        { typeof(decimal), "number" },
        { typeof(decimal?), "number" },
        { typeof(string), "string" },
        { typeof(DateTime), "date" },
        { typeof(DateTime?), "date" },
        { typeof(TimeSpan), "time" },
        { typeof(TimeSpan?), "time" },
        { typeof(DateTimeOffset), "date" },
        { typeof(DateTimeOffset?), "date" },
        { typeof(Guid), "guid" },
        { typeof(Guid?), "guid" },
        { typeof(bool), "bool" },
        { typeof(bool?), "bool" },
        { typeof(double), "number" },
        { typeof(double?), "number" },
        { typeof(float), "number" },
        { typeof(float?), "number" }
    };

    private static string GetMetadataTypeName(Type type)
    {
        if (type == typeof(string))
        {
            return "string";
        }

        if (typeof(IEnumerable).IsAssignableFrom(c: type))
        {
            return "array";
        }

        return TypeNames.TryGetValue(
            key: type,
            value: out string name)
                ? name
                : "object";
    }

    private static string GetCSharpTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        IEnumerable<string> genericNames = type.GenericTypeArguments.Select(
            selector: GetCSharpTypeName);

        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>".Replace(oldValue: "System.Object", newValue: "dynamic");
    }

    private bool IsJoinType(Type type)
    {
        TableAttribute table = attributeBroker.GetCustomAttribute<TableAttribute>(
            memberInfo: type);

        return table != null
            && type.GetProperties().Length == 4
            && type.GetProperties()
                .Where(predicate: property => property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                .All(predicate: property =>
                    attributeBroker.GetCustomAttribute<ForeignKeyAttribute>(
                        memberInfo: property) != null);
    }
}