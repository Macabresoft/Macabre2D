namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// Interface for an entity that can be pixel snapped
/// </summary>
public interface IPixelSnappable {
    /// <summary>
    /// Gets the pixel snap behavior of this entity.
    /// </summary>
    public PixelSnap PixelSnap { get; }
}