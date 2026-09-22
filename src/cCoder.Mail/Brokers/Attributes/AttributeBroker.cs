// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;

namespace cCoder.Mail.Brokers.Attributes;

internal sealed class AttributeBroker : IAttributeBroker
{
    public TAttribute GetCustomAttribute<TAttribute>(
        MemberInfo memberInfo)
        where TAttribute : Attribute =>
        memberInfo.GetCustomAttribute<TAttribute>();
}