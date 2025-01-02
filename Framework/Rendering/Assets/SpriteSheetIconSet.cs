namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
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
    /// Gets or sets the default kerning for this icon set.
    /// </summary>
    public abstract int Kerning { get; set; }

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
    private readonly Dictionary<TKey, float> _characterToWidth = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<TKey, SpriteSheetIcon<TKey>> _keyToIcons = new();

    private int _kerning;

    /// <inheritdoc />
    public override IReadOnlyCollection<SpriteSheetIcon> Icons => this._keyToIcons.Values;

    /// <inheritdoc />
    [DataMember]
    public override int Kerning {
        get => this._kerning;
        set {
            if (this._kerning != value) {
                this._kerning = value;
                this._characterToWidth.Clear();
            }
        }
    }

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

        this._characterToWidth.Remove(key);
        this.RaisePropertyChanged(nameof(SpriteSheetIconSet));
    }

    /// <summary>
    /// Gets the icon's width with kerning taken into account.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="project">The project.</param>
    /// <returns>The width.</returns>
    public float GetIconWidth(TKey key, IGameProject project) {
        var result = 0f;

        if (this.SpriteSheet is { } spriteSheet) {
            if (this._characterToWidth.TryGetValue(key, out var width)) {
                result = width;
            }
            else if (this._keyToIcons.TryGetValue(key, out var icon)) {
                result = (spriteSheet.SpriteSize.X + icon.Kerning + this.Kerning) * project.UnitsPerPixel;
                this._characterToWidth[key] = result;
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the kerning for the icon associated with the <see cref="key" />. This also includes the kerning
    /// applied by <see cref="SpriteSheetIconSet{TKey}" /> and its <see cref="Kerning" /> property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The kerning.</returns>
    public int GetKerning(TKey key) {
        var result = this.Kerning;

        if (this._keyToIcons.TryGetValue(key, out var icon)) {
            result += icon.Kerning;
        }

        return result;
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

        this._characterToWidth.Remove(key);
        this.RaisePropertyChanged(nameof(SpriteSheetIconSet));
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