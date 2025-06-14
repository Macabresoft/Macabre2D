namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Core;

/// <summary>
/// A key for a sprite animation in a <see cref="SpriteAnimationKey" />.
/// </summary>
public abstract class SpriteAnimationKey : PropertyChangedNotifier {
    private Guid _animationId;

    /// <summary>
    /// Gets the name of this animation key.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets or sets the animation identifier.
    /// </summary>
    public Guid AnimationId {
        get => this._animationId;
        set => this.Set(ref this._animationId, value);
    }
}

/// <summary>
/// A single animation a <see cref="SpriteAnimationKey{TKey}" />.
/// </summary>
public class SpriteAnimationKey<TKey> : SpriteAnimationKey where TKey : struct {

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimationKey{TKey}" /> class.
    /// </summary>
    /// <param name="key">The key.</param>
    public SpriteAnimationKey(TKey key) {
        this.Key = key;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    public TKey Key { get; }

    /// <inheritdoc />
    public override string Name => this.Key.ToString() ?? string.Empty;
}