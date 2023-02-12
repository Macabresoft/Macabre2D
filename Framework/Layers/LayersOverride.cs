namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A <see cref="Layers" /> wrapper that can be enabled or disabled.
/// </summary>
[DataContract]
public class LayersOverride : PropertyChangedNotifier {
    private Layers _value;

    /// <summary>
    /// Initializes a new instance of <see cref="LayersOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isEnabled">A value indicating whether or not this is enabled.</param>
    public LayersOverride(Layers value, bool isEnabled) {
        this.Value = value;
        this.IsEnabled = isEnabled;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LayersOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    public LayersOverride(Layers value) : this(value, false) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LayersOverride" />
    /// </summary>
    public LayersOverride() : this(Layers.None) {
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not this is enabled.
    /// </summary>
    [DataMember]
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [DataMember]
    public Layers Value {
        get => this._value;
        set => this.Set(ref this._value, value);
    }
}