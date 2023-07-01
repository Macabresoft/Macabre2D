﻿namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Represents a character in a <see cref="SpriteSheetFont"/>.
/// </summary>
[DataContract]
public class SpriteSheetFontCharacter : PropertyChangedNotifier {
    private int _kerning;
    
    /// <summary>
    /// Gets or sets the sprite index.
    /// </summary>
    [DataMember]
    public byte SpriteIndex { get; set; }
    
    /// <summary>
    /// Gets the character.
    /// </summary>
    [DataMember]
    public char Character { get; set; }

    /// <summary>
    /// Gets or sets the kerning for this character.
    /// </summary>
    [DataMember]
    public int Kerning {
        get => this._kerning;
        set => this.Set(ref this._kerning, value);
    }
}