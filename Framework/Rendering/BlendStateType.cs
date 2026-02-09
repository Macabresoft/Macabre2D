namespace Macabre2D.Framework;

using System;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Enum defining various kinds of <see cref="BlendStateType" />.
/// </summary>
public enum BlendStateType {
    AlphaBlend,
    Additive,
    NonPremultiplied,
    Opaque
}

/// <summary>
/// Extensions for <see cref="BlendStateType" />.
/// </summary>
public static class BlendStateTypeExtensions {
    /// <summary>
    /// Converts from a <see cref="BlendStateType" /> to a <see cref="BlendState" />.
    /// </summary>
    /// <param name="blendStateType">The blend state type.</param>
    /// <returns>The blend state.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Out of range.</exception>
    public static BlendState ToBlendState(this BlendStateType blendStateType) {
        return blendStateType switch {
            BlendStateType.AlphaBlend => BlendState.AlphaBlend,
            BlendStateType.Additive => BlendState.Additive,
            BlendStateType.NonPremultiplied => BlendState.NonPremultiplied,
            BlendStateType.Opaque => BlendState.Opaque,
            _ => throw new ArgumentOutOfRangeException(nameof(blendStateType), blendStateType, null)
        };
    }
}