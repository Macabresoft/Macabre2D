namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

/// <summary>
/// A set of icons corresponding to game pad <see cref="Buttons" />.
/// </summary>
public class GamePadIconSet : SpriteSheetMember {
    /// <summary>
    /// The default name.
    /// </summary>
    public const string DefaultName = "Game Pad Icons";

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<Buttons, byte> _buttonToIndex = new();

    /// <summary>
    /// Sets the sprite for a button.
    /// </summary>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="button">The button.</param>
    public void SetSprite(byte spriteIndex, Buttons button) {
        this._buttonToIndex[button] = spriteIndex;
    }

    /// <summary>
    /// Tries to get the sprite index associated with the provided button.
    /// </summary>
    /// <param name="button">The button.</param>
    /// <param name="index">The index.</param>
    /// <returns>A value indicating whether or not a sprite was found.</returns>
    public bool TryGetSpriteIndex(Buttons button, [NotNullWhen(true)] out byte? index) {
        index = this._buttonToIndex.TryGetValue(button, out var foundIndex) ? foundIndex : null;
        return index != null;
    }

    /// <summary>
    /// Removes the sprite index assigned to a button.
    /// </summary>
    /// <param name="buttons">The button.</param>
    public void UnsetSprite(Buttons buttons) {
        this._buttonToIndex.Remove(buttons);
    }
}