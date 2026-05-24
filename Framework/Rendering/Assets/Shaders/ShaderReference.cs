namespace Macabre2D.Framework;

using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Asset reference for <see cref="ShaderAsset" />.
/// </summary>
[Category(CommonCategories.Shader)]
public class ShaderReference : AssetReference<ShaderAsset, Effect> {

    /// <summary>
    /// Prepares a shader and returns it.
    /// </summary>
    /// <param name="renderSize">The render size.</param>
    /// <param name="game">The game.</param>
    /// <param name="scene">The scene.</param>
    /// <returns>The shader.</returns>
    public Effect? PrepareAndGetShader(Vector2 renderSize, IGame game, IScene scene) {
        if (this.Asset is { Content: { } effect, Configuration: { } configuration }) {
            configuration.FillParameters(effect, renderSize, game, scene);
            return effect;
        }

        return null;
    }
}