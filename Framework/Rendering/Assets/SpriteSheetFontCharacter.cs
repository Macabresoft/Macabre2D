namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// Represents a character in a <see cref="SpriteSheetFont"/>.
/// </summary>
[DataContract]
public class SpriteSheetFontCharacter {
    /// <summary>
    /// Gets or sets the sprite index.
    /// </summary>
    [DataMember]
    public byte SpriteIndex { get; set; }
    
    /// <summary>
    /// Gets or sets the kerning for this character.
    /// </summary>
    [DataMember]
    public int Kerning { get; set; }
    
    // public string CharacterWidth { get; set; } // Set this when SpriteIndex changes, Kerning changes, or when he SpriteSheetFont has its SpriteSize changed
}