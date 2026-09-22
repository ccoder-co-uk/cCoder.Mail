// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;

namespace cCoder.Mail.Brokers.Attributes;

internal interface IAttributeBroker
{
    TAttribute GetCustomAttribute<TAttribute>(MemberInfo memberInfo)
        where TAttribute : Attribute;
}