namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a shader configuration.
/// </summary>
public interface IShaderConfiguration {
    /// <summary>
    /// Fills the effect with parameters.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="renderSize">The render size.</param>
    /// <param name="game">The game.</param>
    /// <param name="scene">The scene.</param>
    void FillParameters(Effect effect, Vector2 renderSize, IGame game, IScene scene);

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="assetManager">The asset manager.</param>
    void Initialize(IAssetManager assetManager);

    /// <summary>
    /// Resets this configuration to defaults.
    /// </summary>
    void ResetToDefault();
}

/// <summary>
/// A base implementation of <see cref="IShaderConfiguration" />.
/// </summary>
[DataContract]
public class ShaderConfiguration : IShaderConfiguration {

    /// <summary>
    /// Gets an empty instance of <see cref="IShaderConfiguration" />.
    /// </summary>
    public static IShaderConfiguration Empty { get; } = new ShaderConfiguration();

    /// <inheritdoc />
    public virtual void FillParameters(Effect effect, Vector2 renderSize, IGame game, IScene scene) {
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="assetManager">The asset manager.</param>
    public virtual void Initialize(IAssetManager assetManager) {
    }

    /// <inheritdoc />
    public virtual void ResetToDefault() {
    }
}