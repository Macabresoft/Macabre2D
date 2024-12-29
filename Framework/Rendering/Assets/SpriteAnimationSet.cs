namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

/// <summary>
/// Base class for <see cref="SpriteAnimation" /> sets as a sprite sheet member.
/// </summary>
public abstract class SpriteAnimationSet : SpriteSheetMember {

    /// <summary>
    /// Gets the animations in this set.
    /// </summary>
    public abstract IReadOnlyCollection<SpriteAnimationKey> Animations { get; }

    /// <summary>
    /// Refreshes the animation keys to update them with any new entries.
    /// </summary>
    public abstract void RefreshAnimationKeys();
}

/// <summary>
/// Base class for <see cref="SpriteAnimation" /> sets as a sprite sheet member.
/// </summary>
/// <typeparam name="TKey">The type of key.</typeparam>
public abstract class SpriteAnimationSet<TKey> : SpriteAnimationSet where TKey : struct {
    private readonly Dictionary<TKey, SpriteAnimation> _keyToAnimation = new();

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<TKey, SpriteAnimationKey<TKey>> _keyToAnimationKey = new();

    /// <inheritdoc />
    public override IReadOnlyCollection<SpriteAnimationKey> Animations => this._keyToAnimationKey.Values;

    /// <summary>
    /// Removes the sprite index assigned to a key.
    /// </summary>
    /// <param name="key">The key.</param>
    public void ClearSprite(TKey key) {
        if (this._keyToAnimationKey.TryGetValue(key, out var animationKey)) {
            animationKey.AnimationId = Guid.Empty;
        }
        else {
            this._keyToAnimationKey[key] = new SpriteAnimationKey<TKey>(key);
        }

        this._keyToAnimation.Remove(key);
    }

    /// <summary>
    /// Sets the animation for a key.
    /// </summary>
    /// <param name="animation">The animation.</param>
    /// <param name="key">The key.</param>
    public void SetAnimation(SpriteAnimation animation, TKey key) {
        if (this._keyToAnimationKey.TryGetValue(key, out var animationKey)) {
            animationKey.AnimationId = animation.Id;
        }
        else {
            this._keyToAnimationKey[key] = new SpriteAnimationKey<TKey>(key) {
                AnimationId = animation.Id
            };
        }

        this._keyToAnimation[key] = animation;
    }

    /// <summary>
    /// Tries to get the sprite index associated with the provided key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="animation">The animation.</param>
    /// <returns>A value indicating whether a sprite was found.</returns>
    public bool TryGetSpriteIndex(TKey key, [NotNullWhen(true)] out SpriteAnimation? animation) {
        if (!this._keyToAnimation.TryGetValue(key, out animation) &&
            this._keyToAnimationKey.TryGetValue(key, out var animationKey) &&
            this.SpriteSheet?.TryGetPackaged(animationKey.AnimationId, out animation) == true) {
            this._keyToAnimation[key] = animation;
        }

        return animation != null;
    }

    /// <summary>
    /// Refreshes the key by adding it to the icon list if it is not there.
    /// </summary>
    /// <param name="key">The key.</param>
    protected void RefreshAnimationKey(TKey key) {
        if (!this._keyToAnimationKey.ContainsKey(key)) {
            this._keyToAnimationKey[key] = new SpriteAnimationKey<TKey>(key);
        }
    }
}