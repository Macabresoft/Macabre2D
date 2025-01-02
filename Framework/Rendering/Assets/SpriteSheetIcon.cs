﻿namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A single icon in a <see cref="SpriteSheetIconSet" />.
/// </summary>
[DataContract]
public abstract class SpriteSheetIcon : PropertyChangedNotifier {
    private int _kerning;
    private byte? _spriteIndex;

    /// <summary>
    /// Gets the name of this icon.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets or sets the kerning for this character.
    /// </summary>
    [DataMember]
    public int Kerning {
        get => this._kerning;
        set => this.Set(ref this._kerning, value);
    }

    /// <summary>
    /// Gets or sets the sprite index.
    /// </summary>
    [DataMember]
    public byte? SpriteIndex {
        get => this._spriteIndex;
        set => this.Set(ref this._spriteIndex, value);
    }
}

/// <summary>
/// A single icon in a <see cref="SpriteSheetIconSet{TKey}" />.
/// </summary>
public class SpriteSheetIcon<TKey> : SpriteSheetIcon where TKey : struct {

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetIcon{TKey}" /> class.
    /// </summary>
    /// <param name="key">The key.</param>
    public SpriteSheetIcon(TKey key) {
        this.Key = key;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    [DataMember]
    public TKey Key { get; }

    /// <inheritdoc />
    public override string Name => this.Key.ToString() ?? string.Empty;
}