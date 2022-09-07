﻿namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// A <see cref="Layers" /> wrapper that can be enabled or disabled.
/// </summary>
[DataContract]
public class LayersOverride : NotifyPropertyChanged {
    private bool _isEnabled = true;
    private Layers _value;

    /// <summary>
    /// Initializes a new instance of <see cref="LayersOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isEnabled">A value indicating whether or not this is enabled.</param>
    public LayersOverride(Layers value, bool isEnabled) {
        this._value = value;
        this._isEnabled = isEnabled;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LayersOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    public LayersOverride(Layers value) : this(value, true) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LayersOverride" />
    /// </summary>
    public LayersOverride() {
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not this is enabled.
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
    public Layers Value {
        get => this._value;
        set => this.Set(ref this._value, value);
    }
}