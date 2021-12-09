namespace Macabresoft.AvaloniaEx;

using System;
using System.Reflection;

public sealed class AttributeMemberInfo<T> where T : Attribute {
    public AttributeMemberInfo(MemberInfo memberInfo, T attribute) {
        this.MemberInfo = memberInfo;
        this.Attribute = attribute;
    }

    public T Attribute { get; }

    public MemberInfo MemberInfo { get; }
}