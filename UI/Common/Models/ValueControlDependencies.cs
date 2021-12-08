namespace Macabresoft.Macabre2D.UI.Common;

using System;

/// <summary>
/// Dependencies for <see cref="ValueControl{T}" />.
/// </summary>
public class ValueControlDependencies {
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueControlDependencies" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="valueType">The value's type.</param>
    /// <param name="valuePropertyName">The value's property name on its parent object.</param>
    /// <param name="title">The title of the value control.</param>
    /// <param name="owner">The owner of the value.</param>
    public ValueControlDependencies(object value, Type valueType, string valuePropertyName, string title, object owner) {
        this.Value = value;
        this.ValueType = valueType;
        this.ValuePropertyName = valuePropertyName;
        this.Title = title;
        this.Owner = owner;
    }

    /// <summary>
    /// Gets the owner of the value.
    /// </summary>
    public object Owner { get; }

    /// <summary>
    /// Gets the title of the value control.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets the value's property name on its parent object.
    /// </summary>
    public string ValuePropertyName { get; }

    /// <summary>
    /// Gets the value's type.
    /// </summary>
    public Type ValueType { get; }
}