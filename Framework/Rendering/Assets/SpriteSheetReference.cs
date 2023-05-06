namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A reference to a <see cref="SpriteSheet" />.
/// </summary>
public class SpriteSheetReference : AssetReference<SpriteSheet, Texture2D> {
    /// <summary>
    /// Gets all the animations for the referenced sprite sheet.
    /// </summary>
    /// <returns>All animations for the referenced sprite sheet.</returns>
    public IEnumerable<SpriteAnimation> GetAnimations() {
        if (this.Asset is { } spriteSheet) {
            return spriteSheet.SpriteAnimations;
        }

        return Enumerable.Empty<SpriteAnimation>();
    }

    /// <summary>
    /// Gets all the auto tile sets for the referenced sprite sheet.
    /// </summary>
    /// <returns>All auto tile sets for the referenced sprite sheet.</returns>
    public IEnumerable<AutoTileSet> GetAutoTileSets() {
        if (this.Asset is { } spriteSheet) {
            return spriteSheet.AutoTileSets;
        }

        return Enumerable.Empty<AutoTileSet>();
    }

    /// <summary>
    /// Gets all the fonts for the referenced sprite sheet.
    /// </summary>
    /// <returns>All fonts for the referenced sprite sheet.</returns>
    public IEnumerable<SpriteSheetFont> GetFonts() {
        if (this.Asset is { } spriteSheet) {
            return spriteSheet.Fonts;
        }

        return Enumerable.Empty<SpriteSheetFont>();
    }
}