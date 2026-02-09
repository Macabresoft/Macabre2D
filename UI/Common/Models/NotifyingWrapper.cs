namespace Macabre2D.UI.Common;

using Macabresoft.Core;
using Macabre2D.Framework;

/// <summary>
/// An wrapper that provides notifications on property changed. Useful for structs in a collection that need to be edited.
/// </summary>
/// <typeparam name="T">The type to wrap.</typeparam>
public class NotifyingWrapper<T> : PropertyChangedNotifier {
    private T _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyingWrapper{T}" /> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    public NotifyingWrapper(T value) {
        this._value = value;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public T Value {
        get => this._value;
        set => this.Set(ref this._value, value);
    }
}