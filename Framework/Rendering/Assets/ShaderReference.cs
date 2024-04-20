namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Asset reference for <see cref="ShaderAsset" />.
/// </summary>
[Category(CommonCategories.Shader)]
public class ShaderReference : AssetReference<ShaderAsset, Effect> {

    /// <inheritdoc />
    public override void Initialize(IAssetManager assetManager) {
        base.Initialize(assetManager);
        this.PrepareAndGetShader();
    }

    /// <summary>
    /// Prepares a shader and returns it.
    /// </summary>
    /// <returns>The shader.</returns>
    public Effect? PrepareAndGetShader() {
        var effect = this.Asset?.Content;
        if (effect != null) {
            this.FillParameters(effect);
        }

        return effect;
    }

    /// <summary>
    /// A method to fill the parameters of the effect.
    /// </summary>
    /// <param name="effect">The effect.</param>
    protected virtual void FillParameters(Effect effect) {
    }
}