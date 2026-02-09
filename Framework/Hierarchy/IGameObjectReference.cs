namespace Macabre2D.Framework;

/// <summary>
/// An interface for referencing game objects that can be resolved through the <see cref="IScene"/>.
/// </summary>
public interface IGameObjectReference {
    /// <summary>
    /// Deinitializes this instance.
    /// </summary>
    void Deinitialize();

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="scene">The scene.</param>
    void Initialize(IScene scene);
}