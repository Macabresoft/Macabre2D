﻿namespace Macabresoft.Macabre2D.Editor.Library.Models {
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Interface for a control that can generically edit values. Generally used for serialized game components.
    /// </summary>
    /// <typeparam name="T">The type being edited.</typeparam>
    public interface IValueEditor<T> : INotifyPropertyChanged {
        
        /// <summary>
        /// Gets or sets the owner of the value. This is only required if not directly binding to the value.
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Gets or sets the value's property name. This is only required if not directly binding to the value.
        /// </summary>
        string ValuePropertyName { get; set; }

        /// <summary>
        /// Called when the value changes if value is not 
        /// </summary>
        event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
    }
}