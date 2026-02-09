namespace Macabre2D.Framework;

using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An entity that renders a <see cref="Texture2D" /> provided at runtime.
/// </summary>
public sealed class Texture2DRenderer : BaseSpriteEntity {

    /// <inheritdoc />
    public override byte? SpriteIndex => 0;

    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    public Texture2D? Texture {
        get;
        set {
            field = value;
            if (field != null) {
                this.SpriteSheet.LoadContent(field);
            }
            else {
                this.SpriteSheet.UnloadContent();
            }
        }
    }

    /// <inheritdoc />
    protected override SpriteSheet SpriteSheet { get; } = new();
}