namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Interface for a sprite sheet icon set.
/// </summary>
public interface ISpriteSheetIconSet {

    /// <summary>
    /// Gets the icons in this set.
    /// </summary>
    IReadOnlyCollection<SpriteSheetIcon> Icons { get; }

    /// <summary>
    /// Refreshes the icons to update them with any new entries.
    /// </summary>
    void RefreshIcons();
}

/// <summary>
/// Base class for icon sets as a sprite sheet member.
/// </summary>
/// <typeparam name="TKey">The key.</typeparam>
public abstract class SpriteSheetIconSet<TKey> : SpriteSheetKeyedMember<TKey>, ISpriteSheetIconSet where TKey : struct {
    [DataMember]
    private readonly List<SpriteSheetIcon<TKey>> _icons = new();

    /// <inheritdoc />
    public IReadOnlyCollection<SpriteSheetIcon> Icons => this._icons;

    /// <summary>
    /// Removes the sprite index assigned to a key.
    /// </summary>
    /// <param name="key">The key.</param>
    public override void ClearSprite(TKey key) {
        if (this._icons.FirstOrDefault(x => x.Key.Equals(key)) is { } icon) {
            icon.SpriteIndex = null;
        }
        else {
            icon = new SpriteSheetIcon<TKey>(key);
            this._icons.Add(icon);
        }
    }

    /// <inheritdoc />
    public abstract void RefreshIcons();

    /// <summary>
    /// Sets the sprite for a key.
    /// </summary>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="key">The key.</param>
    public override void SetSprite(byte spriteIndex, TKey key) {
        if (this._icons.FirstOrDefault(x => x.Key.Equals(key)) is { } icon) {
            icon.SpriteIndex = spriteIndex;
        }
        else {
            this._icons.Add(new SpriteSheetIcon<TKey>(key) {
                SpriteIndex = spriteIndex
            });
        }
    }

    /// <summary>
    /// Tries to get the sprite index associated with the provided key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="index">The index.</param>
    /// <returns>A value indicating whether a sprite was found.</returns>
    public bool TryGetSpriteIndex(TKey key, [NotNullWhen(true)] out byte? index) {
        index = this._icons.FirstOrDefault(x => x.Key.Equals(key))?.SpriteIndex;
        return index != null;
    }

    /// <summary>
    /// Refreshes the key by adding it to the icon list if it is not there.
    /// </summary>
    /// <param name="key">The key.</param>
    protected void RefreshIcon(TKey key) {
        if (!this._icons.Any(x => x.Key.Equals(key))) {
            this._icons.Add(new SpriteSheetIcon<TKey>(key));
        }
    }
}