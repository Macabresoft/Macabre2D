namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// A base class for <see cref="SpriteSheetMember" /> classes that operate as a dynamic dictionary with keys being related to sprite indexes.
/// </summary>
/// <typeparam name="TKey">The key this member uses to retrieve the appropriate sprite index.</typeparam>
public abstract class SpriteSheetKeyedMember<TKey> : SpriteSheetMember where TKey : struct {
    /// <summary>
    /// Clears the sprite index for the given key.
    /// </summary>
    /// <param name="key">The key.</param>
    public abstract void ClearSprite(TKey key);

    /// <summary>
    /// Sets the sprite index for a given key.
    /// </summary>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="key">The key.</param>
    public abstract void SetSprite(byte spriteIndex, TKey key);
}