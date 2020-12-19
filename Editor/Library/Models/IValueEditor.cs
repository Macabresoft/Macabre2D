namespace Macabresoft.Macabre2D.Editor.Library.Models {
    using System;
    using Avalonia;

    /// <summary>
    /// A basic value untyped version of <see cref="IValueEditor{T}" />
    /// </summary>
    public interface IValueEditor : IAvaloniaObject {
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

        /// <summary>
        /// Called when the value changes if value is not
        /// </summary>
        event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
    }
}