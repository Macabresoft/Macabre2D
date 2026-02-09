namespace Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A reference to a sprite on a <see cref="SpriteSheet" />.
/// </summary>
public class SpriteReference : AssetReference<SpriteSheet, Texture2D> {

    /// <summary>
    /// Gets or sets the sprite index on a <see cref="SpriteSheet" />. The sprite sheet is read from left to right, top to
    /// bottom.
    /// </summary>
    [DataMember]
    public byte SpriteIndex {
        get;
        set => this.Set(ref field, value);
    }

    /// <inheritdoc />
    public override void Clear() {
        base.Clear();
        this.SpriteIndex = 0;
    }

    /// <inheritdoc />
    protected override void OnAssetPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnAssetPropertyChanged(sender, e);

        if (e.PropertyName is nameof(SpriteSheet.Rows) or nameof(SpriteSheet.Columns) && sender is SpriteSheet spriteSheet) {
            if (this.SpriteIndex > spriteSheet.Rows * spriteSheet.Columns) {
                this.SpriteIndex = 0;
            }
        }
    }
}