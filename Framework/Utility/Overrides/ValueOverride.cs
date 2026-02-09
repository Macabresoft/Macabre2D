namespace Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Class for values that can be optionally overriden.
/// </summary>
[DataContract]
public abstract class ValueOverride<T> : PropertyChangedNotifier where T : notnull {
    private bool _isEnabled;
    private T _value;

    /// <summary>
    /// Initializes a new instance of <see cref="ValueOverride{T}" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isEnabled">A value indicating whether this is enabled.</param>
    protected ValueOverride(T value, bool isEnabled) {
        this._value = value;
        this._isEnabled = isEnabled;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ValueOverride{T}" />
    /// </summary>
    /// <param name="value">The value.</param>
    protected ValueOverride(T value) : this(value, false) {
    }

    /// <summary>
    /// Gets or sets a value indicating whether this is enabled.
    /// </summary>
    [DataMember]
    public bool IsEnabled {
        get => this._isEnabled;
        set => this.Set(ref this._isEnabled, value);
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [DataMember]
    public T Value {
        get => this._value;
        set => this.Set(ref this._value, value);
    }
}