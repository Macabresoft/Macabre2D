namespace Macabre2D.Framework;

using Microsoft.Xna.Framework;

/// <summary>
/// A value override for <see cref="Color" />.
/// </summary>
public class ColorOverride : ValueOverride<Color> {
    /// <summary>
    /// Initializes a new instance of <see cref="ColorOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isEnabled">A value indicating whether or not this is enabled.</param>
    public ColorOverride(Color value, bool isEnabled) : base(value, isEnabled) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ColorOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    public ColorOverride(Color value) : this(value, false) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ColorOverride" />
    /// </summary>
    public ColorOverride() : this(Color.White) {
    }
}