namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// A font based on a sprite sheet.
/// </summary>
public class SpriteSheetFont : SpriteSheetMember {
    /// <summary>
    /// The default name.
    /// </summary>
    public const string DefaultName = "Font";

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<byte, SpriteSheetFontCharacter> _characterIndexToCharacter = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<char, byte> _characterToIndex = new();

    /// <summary>
    /// Sets the sprite for the specified character
    /// </summary>
    /// <param name="spriteIndex">The sprite.</param>
    /// <param name="character">The character.</param>
    public void SetSprite(byte spriteIndex, char character) {
        if (!this._characterToIndex.TryGetValue(character, out var index)) {
            index = (byte)this._characterToIndex.Count;
            this._characterToIndex[character] = index;
        }

        if (this._characterIndexToCharacter.TryGetValue(index, out var spriteCharacter)) {
            spriteCharacter.SpriteIndex = spriteIndex;
        }
        else {
            this._characterIndexToCharacter[index] = new SpriteSheetFontCharacter() {
                SpriteIndex = spriteIndex
            };
        }

        this.RaisePropertyChanged(nameof(SpriteSheetFont));
    }

    /// <summary>
    /// Tries to get the sprite index associated with the specified character.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <returns>A value indicating whether or not the sprite was found.</returns>
    public bool TryGetSpriteIndex(char character, out byte spriteIndex) {
        spriteIndex = 0;
        if (this._characterToIndex.TryGetValue(character, out var index) && this._characterIndexToCharacter.TryGetValue(index, out var spriteCharacter)) {
            spriteIndex = spriteCharacter.SpriteIndex;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes the sprite index for the given character, effectively blanking it out.
    /// </summary>
    /// <param name="character">The character.</param>
    public void UnsetSprite(char character) {
        if (this._characterToIndex.TryGetValue(character, out var index)) {
            this._characterToIndex.Remove(character);
            this._characterIndexToCharacter.Remove(index);
        }

        this.RaisePropertyChanged(nameof(SpriteSheetFont));
    }
}