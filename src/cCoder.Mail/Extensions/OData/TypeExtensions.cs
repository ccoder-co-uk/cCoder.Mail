// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Collections;
using cCoder.Mail.Models.OData;

namespace cCoder.Mail.Extensions.OData;

internal static class TypeExtensions
{
    internal static MetadataContainer CreateMetadataContainer(
        this Type type,
        bool isEntity = false,
        bool hasEndpoint = false)
    {
        bool isValueType = type.IsValueType || type == typeof(string);

        return new MetadataContainer
        {
            IsValueType = isValueType,
            Type = type.GetMetadataTypeName(),
            Name = type.Name,
            DisplayName = type.Name,
            Description = type.Name,
            ServerType = type.AssemblyQualifiedName,
            ServerTypeName = type.GetCSharpTypeName(),
            Properties = isValueType
                ? []
                : type.GetProperties()
                    .Select(selector: property => property.CreatePropertyContainer())
                    .ToArray(),
            IsEntity = isEntity,
            IsJoinEntity = isEntity && type.IsJoinType(),
            HasEndpoint = hasEndpoint,
        };
    }

    internal static ExtendedMetadataContainer CreateExtendedMetadataContainer(
        this Type type,
        bool isEntity = false,
        bool hasEndpoint = false)
    {
        MetadataContainer metadata = type.CreateMetadataContainer(
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

    internal static string GetMetadataTypeName(this Type type)
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

    internal static string GetCSharpTypeName(this Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        IEnumerable<string> genericNames = type.GenericTypeArguments.Select(selector: argument => argument.GetCSharpTypeName());
        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>".Replace(oldValue: "System.Object", newValue: "dynamic");
    }

    internal static bool IsJoinType(this Type type)
    {
        TableAttribute table = type.GetCustomAttribute<TableAttribute>();

        return table != null
            && type.GetProperties().Length == 4
            && type.GetProperties()
                .Where(predicate: property => property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                .All(predicate: property => property.GetCustomAttribute<ForeignKeyAttribute>() != null);
    }

    internal static PropertyInfo GetIdProperty(this Type type)
    {
        if (!type.IsJoinType())
        {
            PropertyInfo idProperty =
                type.GetProperty(name: "ID")
                ?? type.GetProperty(name: "Id")
                ?? type.GetProperty(name: type.Name + "Id")
                ?? type.GetProperty(name: type.Name + "ID")
                ?? type.GetProperties()
                .FirstOrDefault(predicate: property =>
                    property.GetCustomAttributes(attributeType: typeof(KeyAttribute), inherit: false)
                .Any());

            if (idProperty != null)
            {
                return idProperty;
            }
        }
        else
        {
            return new CompositePropertyInfo(type: type);
        }

        return null;
    }
}