namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Represents a character in a <see cref="SpriteSheetFont"/>.
/// </summary>
[DataContract]
public sealed class SpriteSheetFontCharacter : PropertyChangedNotifier {
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
    public int Kerning { get; set; }
}