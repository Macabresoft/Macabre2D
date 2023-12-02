namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

/// <summary>
/// Base class for icon sets as a sprite sheet member.
/// </summary>
/// <typeparam name="TKey">The key.</typeparam>
public abstract class BaseIconSet<TKey> : SpriteSheetKeyedMember<TKey> where TKey : struct {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<TKey, byte> _keyToIndex = new();

    /// <summary>
    /// Removes the sprite index assigned to a key.
    /// </summary>
    /// <param name="key">The key.</param>
    public override void ClearSprite(TKey key) {
        this._keyToIndex.Remove(key);
    }

    /// <summary>
    /// Sets the sprite for a key.
    /// </summary>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="key">The key.</param>
    public override void SetSprite(byte spriteIndex, TKey key) {
        this._keyToIndex[key] = spriteIndex;
    }

    /// <summary>
    /// Tries to get the sprite index associated with the provided key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="index">The index.</param>
    /// <returns>A value indicating whether or not a sprite was found.</returns>
    public bool TryGetSpriteIndex(TKey key, [NotNullWhen(true)] out byte? index) {
        index = this._keyToIndex.TryGetValue(key, out var foundIndex) ? foundIndex : null;
        return index != null;
    }
}