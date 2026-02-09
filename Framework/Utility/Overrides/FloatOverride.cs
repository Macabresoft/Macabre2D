namespace Macabre2D.Framework;

/// <summary>
/// A <see cref="byte" /> wrapper that can be enabled or disabled.
/// </summary>
public class FloatOverride : ValueOverride<float> {

    /// <summary>
    /// Initializes a new instance of <see cref="FloatOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isEnabled">A value indicating whether this is enabled.</param>
    public FloatOverride(float value, bool isEnabled) : base(value, isEnabled) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FloatOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    public FloatOverride(float value) : base(value) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FloatOverride" />
    /// </summary>
    public FloatOverride() : this(0f) {
    }
}