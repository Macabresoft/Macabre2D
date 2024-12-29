namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

/// <summary>
/// Base class for icon sets as a sprite sheet member.
/// </summary>
public abstract class SpriteSheetIconSet : SpriteSheetMember {
    /// <summary>
    /// Gets the icons in this set.
    /// </summary>
    public abstract IReadOnlyCollection<SpriteSheetIcon> Icons { get; }

    /// <summary>
    /// Refreshes the icons to update them with any new entries.
    /// </summary>
    public abstract void RefreshIcons();
}

/// <summary>
/// Base class for icon sets as a sprite sheet member.
/// </summary>
/// <typeparam name="TKey">The type of key.</typeparam>
public abstract class SpriteSheetIconSet<TKey> : SpriteSheetIconSet where TKey : struct {

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<TKey, SpriteSheetIcon<TKey>> _keyToIcons = new();

    /// <inheritdoc />
    public override IReadOnlyCollection<SpriteSheetIcon> Icons => this._keyToIcons.Values;

    /// <summary>
    /// Removes the sprite index assigned to a key.
    /// </summary>
    /// <param name="key">The key.</param>
    public void ClearSprite(TKey key) {
        if (this._keyToIcons.TryGetValue(key, out var icon)) {
            icon.SpriteIndex = null;
        }
        else {
            this._keyToIcons[key] = new SpriteSheetIcon<TKey>(key);
        }
    }

    /// <summary>
    /// Sets the sprite for a key.
    /// </summary>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="key">The key.</param>
    public void SetSprite(byte spriteIndex, TKey key) {
        if (this._keyToIcons.TryGetValue(key, out var icon)) {
            icon.SpriteIndex = spriteIndex;
        }
        else {
            this._keyToIcons[key] = new SpriteSheetIcon<TKey>(key) {
                SpriteIndex = spriteIndex
            };
        }
    }

    /// <summary>
    /// Tries to get the sprite index associated with the provided key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="index">The index.</param>
    /// <returns>A value indicating whether a sprite was found.</returns>
    public bool TryGetSpriteIndex(TKey key, [NotNullWhen(true)] out byte? index) {
        index = this._keyToIcons.TryGetValue(key, out var icon) ? icon.SpriteIndex : null;
        return index != null;
    }

    /// <summary>
    /// Refreshes the key by adding it to the icon list if it is not there.
    /// </summary>
    /// <param name="key">The key.</param>
    protected void RefreshIcon(TKey key) {
        if (!this._keyToIcons.ContainsKey(key)) {
            this._keyToIcons[key] = new SpriteSheetIcon<TKey>(key);
        }
    }
}