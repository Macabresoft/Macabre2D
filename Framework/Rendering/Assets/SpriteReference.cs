namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabre2D.Common;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A reference to a sprite on a <see cref="SpriteSheet" />.
/// </summary>
public class SpriteReference : AssetReference<SpriteSheet, Texture2D> {
    private byte _spriteIndex;

    /// <summary>
    /// Gets or sets the sprite index on a <see cref="SpriteSheet" />. The sprite sheet is read from left to right, top to
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

    /// <summary>
    /// Sets the sprite reference to a new sprite index and new content identifier without calling multiple events in the process.
    /// </summary>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    public void SetSprite(Guid spriteSheetId, byte spriteIndex) {
        this._spriteIndex = spriteIndex;
        this.ContentId = spriteSheetId;
    }

    /// <summary>
    /// Sets the sprite reference to a new sprite index and new content identifier without calling multiple events in the process.
    /// </summary>
    /// <param name="spriteReference">The sprite reference that this instance should match.</param>
    public void SetSpriteReference(SpriteReference spriteReference) {
        this.SetSprite(spriteReference.ContentId, spriteReference.SpriteIndex);
    }

    /// <summary>
    /// Sets the sprite reference to a new sprite index and new content identifier without calling multiple events in the process.
    /// </summary>
    /// <param name="spriteReference">The sprite reference that this instance should match.</param>
    public void SetSpriteReference(SpriteSheetSpriteIndexReference spriteReference) {
        this.SetSprite(spriteReference.SpriteSheetId, spriteReference.SpriteIndex);
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