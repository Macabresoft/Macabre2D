namespace Macabre2D.Framework;

/// <summary>
/// A <see cref="byte" /> wrapper that can be enabled or disabled.
/// </summary>
public class ByteOverride : ValueOverride<byte> {
    /// <summary>
    /// Initializes a new instance of <see cref="ByteOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isEnabled">A value indicating whether this is enabled.</param>
    public ByteOverride(byte value, bool isEnabled) : base(value, isEnabled) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ByteOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    public ByteOverride(byte value) : this(value, false) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ByteOverride" />
    /// </summary>
    public ByteOverride() : this(0) {
    }
}