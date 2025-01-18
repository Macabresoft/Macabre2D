namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A base class for rendering a <see cref="SpriteSheetIconSet{TKey}" />.
/// </summary>
/// <typeparam name="TKey">The type of key.</typeparam>
/// <typeparam name="TIconSet">The type of icon set.</typeparam>
/// <typeparam name="TIconSetReference">The type of icon set reference.</typeparam>
public abstract class IconSetRenderer<TKey, TIconSet, TIconSetReference> : BaseSpriteEntity where TKey : struct where TIconSet : SpriteSheetIconSet<TKey> where TIconSetReference : SpriteSheetAssetReference<TIconSet>, new() {
    private int _currentKerning;
    private int _kerning;
    private TKey _key;
    private byte? _spriteIndex;
    private SpriteSheet? _spriteSheet;

    /// <summary>
    /// Gets the icon set reference to render.
    /// </summary>
    [DataMember]
    public TIconSetReference IconSetReference { get; } = new();

    /// <inheritdoc />
    public override byte? SpriteIndex => this._spriteIndex;

    /// <summary>
    /// Gets or sets the kerning. This is the space between letters in pixels. Positive numbers will increase the space, negative numbers will decrease it.
    /// </summary>
    [DataMember]
    public int Kerning {
        get => this._kerning;
        set {
            if (value != this._kerning) {
                this._kerning = value;
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets the input action to display.
    /// </summary>
    [DataMember]
    public TKey Key {
        get => this._key;
        set {
            if (!value.Equals(this._key)) {
                this._key = value;
                this.ResetSprite();
            }
        }
    }

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this._spriteSheet;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        this.IconSetReference.AssetChanged -= this.IconSetReference_AssetChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.IconSetReference.AssetChanged += this.IconSetReference_AssetChanged;
        this.ResetSprite();
    }

    protected override Vector2 CreateSize() {
        if (this.SpriteSheet is { } spriteSheet) {
            return new Vector2(spriteSheet.SpriteSize.X + this._currentKerning, spriteSheet.SpriteSize.Y);
        }

        return base.CreateSize();
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.IconSetReference;
    }

    private void IconSetReference_AssetChanged(object? sender, bool hasAsset) {
        this.ResetSprite();
    }

    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetSprite();
        }
    }

    private void ResetSprite() {
        this._currentKerning = this.Kerning;
        this._spriteIndex = null;
        this._spriteSheet = null;

        if (this.IconSetReference.PackagedAsset is { } iconSet && iconSet.TryGetSpriteIndex(this.Key, out var index)) {
            this._spriteIndex = index;
            this._spriteSheet = iconSet.SpriteSheet;
            this._currentKerning = iconSet.GetKerning(this.Key);
        }

        this.Reset();
    }
}