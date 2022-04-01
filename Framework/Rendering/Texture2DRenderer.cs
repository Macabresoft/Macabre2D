namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An entity that renders a <see cref="Texture2D" /> provided at runtime.
/// </summary>
public sealed class Texture2DRenderer : BaseSpriteEntity {
    private Texture2D? _texture;

    /// <inheritdoc />
    public override byte? SpriteIndex => 0;

    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    public Texture2D? Texture {
        get => this._texture;
        set {
            this._texture = value;
            if (this._texture != null) {
                this.SpriteSheet.LoadContent(this._texture);
            }
            else {
                this.SpriteSheet.UnloadContent();
            }
        }
    }

    /// <inheritdoc />
    protected override SpriteSheetAsset SpriteSheet { get; } = new();
}