namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
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

    private readonly Dictionary<char, float> _characterToWidth = new();

    [DataMember]
    private string _characterLayout = @"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.?!,-=+: ";

    private int _kerning;

    /// <summary>
    /// Gets the font characters and their settings.
    /// </summary>
    public IReadOnlyCollection<SpriteSheetFontCharacter> FontCharacters => this._characterIndexToCharacter.Values;

    /// <summary>
    /// The character layout of this sprite sheet.
    /// </summary>
    public string CharacterLayout {
        get => this._characterLayout;
        set {
            this._characterLayout = new string(value.Distinct().ToArray());

            foreach (var character in this._characterToIndex.Keys.Where(x => !this._characterLayout.Contains(x))) {
                this.UnsetSprite(character);
            }
        }
    }

    /// <summary>
    /// Gets or sets the default kerning for this.
    /// </summary>
    [DataMember]
    public int Kerning {
        get => this._kerning;
        set {
            if (this._kerning != value) {
                this._kerning = value;
                this._characterToWidth.Clear();
            }
        }
    }

    /// <summary>
    /// Clears the sprites.
    /// </summary>
    public void ClearSprites() {
        this._characterToIndex.Clear();
        this._characterIndexToCharacter.Clear();
        this._characterToWidth.Clear();
    }

    /// <summary>
    /// Gets the character's width with kerning taken into account.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="additionalKerning">Additional kerning.</param>
    /// <param name="project">The project.</param>
    /// <returns>The width.</returns>
    public float GetCharacterWidth(SpriteSheetFontCharacter character, int additionalKerning, IGameProject project) {
        var result = 0f;

        if (this.SpriteSheet is { } spriteSheet) {
            if (additionalKerning != 0) {
                if (!this._characterToWidth.TryGetValue(character.Character, out var width)) {
                    width = (spriteSheet.SpriteSize.X + character.Kerning + this.Kerning) * project.UnitsPerPixel;
                    this._characterToWidth[character.Character] = width;
                }

                result = width + additionalKerning * project.UnitsPerPixel;
            }
            else if (this._characterToWidth.TryGetValue(character.Character, out var width)) {
                result = width;
            }
            else {
                result = (spriteSheet.SpriteSize.X + character.Kerning + this.Kerning) * project.UnitsPerPixel;
                this._characterToWidth[character.Character] = result;
            }
        }

        return result;
    }

    /// <summary>
    /// Sets the sprite for the specified character
    /// </summary>
    /// <param name="spriteIndex">The sprite.</param>
    /// <param name="character">The character.</param>
    /// <param name="kerning">The kerning.</param>
    public void SetSprite(byte spriteIndex, char character, int kerning) {
        if (!this._characterToIndex.TryGetValue(character, out var index)) {
            index = (byte)this._characterToIndex.Count;
            this._characterToIndex[character] = index;
        }

        if (this._characterIndexToCharacter.TryGetValue(index, out var spriteCharacter)) {
            spriteCharacter.Character = character;
            spriteCharacter.Kerning = kerning;
            spriteCharacter.SpriteIndex = spriteIndex;
        }
        else {
            this._characterIndexToCharacter[index] = new SpriteSheetFontCharacter {
                Character = character,
                Kerning = kerning,
                SpriteIndex = spriteIndex
            };
        }

        this.RaisePropertyChanged(nameof(SpriteSheetFont));
    }

    /// <summary>
    /// Tries to get the sprite character associated with the specified character.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="spriteSheetCharacter">The sprite sheet character.</param>
    /// <returns>A value indicating whether or not the sprite was found.</returns>
    public bool TryGetSpriteCharacter(char character, [NotNullWhen(true)] out SpriteSheetFontCharacter? spriteSheetCharacter) {
        if (this._characterToIndex.TryGetValue(character, out var index) && this._characterIndexToCharacter.TryGetValue(index, out var spriteCharacter)) {
            spriteSheetCharacter = spriteCharacter;
        }
        else {
            spriteSheetCharacter = null;
        }

        return spriteSheetCharacter != null;
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