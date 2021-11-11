namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using Avalonia;

    /// <summary>
    /// A control for values being displayed in the editor.
    /// </summary>
    public interface IValueControl : IAvaloniaObject {
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// Gets or sets the collection to which this editor belongs.
        /// </summary>
        ValueControlCollection Collection { get; set; }

        /// <summary>
        /// Gets or sets the owner of the value. This is only required if not directly binding to the value.
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        string Title { get; set; }
    }
}