namespace Macabresoft.Macabre2D.Framework;

using System;

/// <summary>
/// Specifies that a <see cref="Type" /> property/field should be restricted to types that can be assigned to the specified type in this attribute.
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TypeRestrictionAttribute : Attribute {
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeRestrictionAttribute" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public TypeRestrictionAttribute(Type type) {
        this.Type = type;
    }

    /// <summary>
    /// Gets the type to which this property is restricted.
    /// </summary>
    public Type Type { get; }
}