namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Macabresoft.Core;

/// <summary>
/// A basic value untyped version of <see cref="IValueEditor{T}" />
/// </summary>
public interface IValueEditor : IValueControl {
    /// <summary>
    /// Called when the value changes.
    /// </summary>
    event EventHandler<ValueChangedEventArgs<object>> ValueChanged;

    /// <summary>
    /// Gets the value's property name. This is only required if not directly binding to the value.
    /// </summary>
    string ValuePropertyName { get; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="newValue">The new value.</param>
    void SetValue(object newValue);

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="newValue">The new value.</param>
    /// <param name="ignoreUpdates">
    /// A value indicating whether the control should ignore update notifications for this
    /// operation.
    /// </param>
    void SetValue(object newValue, bool ignoreUpdates);
}

/// <summary>
/// Interface for a control that can generically edit values. Generally used for serialized game components.
/// </summary>
/// <typeparam name="T">The type being edited.</typeparam>
public interface IValueEditor<T> : IValueEditor {
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    T Value { get; set; }
}