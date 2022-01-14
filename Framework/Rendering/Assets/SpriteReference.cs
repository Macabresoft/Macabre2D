namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// A reference to a sprite on a <see cref="SpriteSheetAsset" />.
/// </summary>
public class SpriteReference : AssetReference<SpriteSheetAsset> {
    private byte _spriteIndex;

    /// <summary>
    /// Gets or sets the sprite index on a <see cref="SpriteSheetAsset" />. The sprite sheet is read from left to right, top to
    /// bottom.
    /// </summary>
    [DataMember]
    public byte SpriteIndex {
        get => this._spriteIndex;
        set => this.Set(ref this._spriteIndex, value);
    }

    /// <inheritdoc />
    public override void Clear() {
        base.Clear();
        this.SpriteIndex = 0;
    }

    /// <inheritdoc />
    protected override void OnAssetPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnAssetPropertyChanged(sender, e);

        if (e.PropertyName is nameof(SpriteSheetAsset.Rows) or nameof(SpriteSheetAsset.Columns) && sender is SpriteSheetAsset spriteSheet) {
            if (this.SpriteIndex > spriteSheet.Rows * spriteSheet.Columns) {
                this.SpriteIndex = 0;
            }
        }
    }
}