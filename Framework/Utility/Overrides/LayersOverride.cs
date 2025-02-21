namespace Macabresoft.Macabre2D.Framework;

using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// A <see cref="Layers" /> wrapper that can be enabled or disabled.
/// </summary>
public class LayersOverride : ValueOverride<Layers> {
    /// <summary>
    /// Initializes a new instance of <see cref="LayersOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isEnabled">A value indicating whether or not this is enabled.</param>
    public LayersOverride(Layers value, bool isEnabled) : base(value, isEnabled) {
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
}