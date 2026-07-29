// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using cCoder.Mail.Models.OData;

namespace cCoder.Mail.Extensions.OData;

internal static class PropertyInfoExtensions
{
    internal static PropertyContainer CreatePropertyContainer(
        this PropertyInfo property) =>
        new()
        {
            Name = property.Name,
            Type = property.PropertyType.GetMetadataTypeName(),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = property.PropertyType.GetCSharpTypeName(),
            IsValueType = property.PropertyType.IsValueType
                || property.PropertyType == typeof(string),
            DisplayName = property.Name,
            ShortDisplayName = property.Name,
            Description = property.Name,
            IsReadOnly = !property.CanWrite,
            Template = property.GetCustomAttribute<KeyAttribute>() is not null
                || property.Name == "Id"
                    ? "key"
                    : property.Name,
            IsRequired = (!(property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition()
                        == typeof(Nullable<>))
                    && property.PropertyType.IsValueType)
                || property.GetCustomAttribute<RequiredAttribute>()
                    is not null
        };
}