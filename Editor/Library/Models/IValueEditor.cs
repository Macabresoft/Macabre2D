namespace Macabresoft.Macabre2D.Editor.Library.Models {
    using System;
    using Avalonia;

    /// <summary>
    /// A basic value untyped version of <see cref="IValueEditor{T}" />
    /// </summary>
    public interface IValueEditor : IAvaloniaObject {
        /// <summary>
        /// Called when the value changes.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<object>> ValueChanged;
        
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// Gets or sets the owner of the value. This is only required if not directly binding to the value.
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the value's property name. This is only required if not directly binding to the value.
        /// </summary>
        string ValuePropertyName { get; set; }

        /// <summary>
        /// Gets or sets the value's type.
        /// </summary>
        Type ValueType { get; set; }

        /// <summary>
        /// Initializes the value editor.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueType"></param>
        /// <param name="valuePropertyName">The name of the value's property/field on the owner object.</param>
        /// <param name="title">The title of this control.</param>
        /// <param name="owner">The owner.</param>
        void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        void SetValue(object newValue);
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
}