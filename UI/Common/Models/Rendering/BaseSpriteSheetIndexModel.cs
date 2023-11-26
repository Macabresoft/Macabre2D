namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// Base class for individual sprites in a <see cref="SpriteSheetMember" />
/// </summary>
public abstract class BaseSpriteSheetIndexModel<TMember, TKey> : PropertyChangedNotifier where TMember : SpriteSheetKeyedMember<TKey> {
    private bool _isInitialized;
    private byte? _spriteIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpriteSheetIndexModel{TMember, TKey}" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="key">The key.</param>
    protected BaseSpriteSheetIndexModel(TMember member, TKey key) {
        this.Member = member;
        this.Key = key;
    }

    /// <summary>
    /// Gets the key to obtain a sprite index from the <see cref="SpriteSheetMember" />.
    /// </summary>
    public TKey Key { get; }

    /// <summary>
    /// Gets or sets the sprite index.
    /// </summary>
    public byte? SpriteIndex {
        get => this._spriteIndex;
        set {
            if (this.Set(ref this._spriteIndex, value)) {
                if (this._spriteIndex == null) {
                    this.OnClearIndex();
                }
                else {
                    this.OnSetIndex(this._spriteIndex.Value);
                }
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="SpriteSheetMember" /> that is being edited.
    /// </summary>
    protected TMember Member { get; }

    /// <summary>
    /// Initializes the sprite index.
    /// </summary>
    /// <param name="spriteIndex"></param>
    protected void InitializeSpriteIndex(byte? spriteIndex) {
        if (!this._isInitialized) {
            try {
                this._spriteIndex = spriteIndex;
            }
            finally {
                this._isInitialized = true;
            }
        }
    }

    /// <summary>
    /// Called when the <see cref="SpriteIndex" /> is changed to anything other than null.
    /// </summary>
    /// <param name="index">The index.</param>
    protected virtual void OnSetIndex(byte index) {
        this.Member.SetSprite(index, this.Key);
    }

    private void OnClearIndex() {
        this.Member.ClearSprite(this.Key);
    }
}