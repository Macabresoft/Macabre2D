namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// An enum that describes various kinds of pixel snapping.
/// </summary>
public enum PixelSnap {
    /// <summary>
    /// The entity will inherit pixel snapping from <see cref="IGameSettings" />.
    /// </summary>
    Inherit,

    /// <summary>
    /// The entity will not pixel snap, regardless of <see cref="IGameSettings" />.
    /// </summary>
    No,

    /// <summary>
    /// The entity will pixel snap, regardless of <see cref="IGameSettings" />.
    /// </summary>
    Yes
}