namespace Macabresoft.Macabre2D.UI.Common {
    /// <summary>
    /// Interface for a control that can generically display information on a value.
    /// </summary>
    /// <typeparam name="T">The type being edited.</typeparam>
    public interface IValueInfo<T> : IValueControl {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        T Value { get; set; }
    }
}