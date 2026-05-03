namespace Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A definition for a project font that includes the sprite sheet and font identifier.
/// </summary>
[DataContract]
public readonly record struct ProjectFontDefinition {
    /// <summary>
    /// Gets an empty definition.
    /// </summary>
    public static readonly ProjectFontDefinition Empty = new();

    /// <summary>
    /// The font identifier which references a <see cref="SpriteSheetFont" />.
    /// </summary>
    [DataMember]
    public readonly Guid FontId;

    /// <summary>
    /// The legacy font identifier which references a <see cref="SpriteFont" />.
    /// </summary>
    [DataMember]
    public readonly Guid LegacyFontId;

    /// <summary>
    /// The sprite sheet identifier which references a <see cref="SpriteSheetFont" />.
    /// </summary>
    [DataMember]
    public readonly Guid SpriteSheetId;

    /// <summary>
    /// The legacy font scaling.
    /// </summary>
    [DataMember]
    public readonly float LegacyFontScale = 1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontDefinition" /> class.
    /// </summary>
    public ProjectFontDefinition() : this(Guid.Empty, Guid.Empty, Guid.Empty, 1f) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontDefinition" /> class.
    /// </summary>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    /// <param name="fontId">The font identifier.</param>
    /// <param name="legacyFontId">The legacy font identifier.</param>
    /// <param name="legacyFontScale">The legacy font scale.</param>
    public ProjectFontDefinition(Guid spriteSheetId, Guid fontId, Guid legacyFontId, float legacyFontScale) {
        this.SpriteSheetId = spriteSheetId;
        this.FontId = fontId;
        this.LegacyFontId = legacyFontId;
        this.LegacyFontScale = legacyFontScale;
    }

    /// <summary>
    /// Creates a new <see cref="ProjectFontDefinition" /> with a different legacy font, while maintaining the original sprite sheet font.
    /// </summary>
    /// <param name="legacyFontId">The legacy font identifier.</param>
    /// <returns>A modified <see cref="ProjectFontDefinition" />.</returns>
    public ProjectFontDefinition WithLegacyFont(Guid legacyFontId) => new(this.SpriteSheetId, this.FontId, legacyFontId, this.LegacyFontScale);

    /// <summary>
    /// Creates a new <see cref="ProjectFontDefinition" /> with a different sprite sheet font, while maintaining the original MonoGame font.
    /// </summary>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    /// <param name="fontId">The font identifier.</param>
    /// <returns>A modified <see cref="ProjectFontDefinition" />.</returns>
    public ProjectFontDefinition WithSpriteSheetFont(Guid spriteSheetId, Guid fontId) => new(spriteSheetId, fontId, this.LegacyFontId, this.LegacyFontScale);

    /// <summary>
    /// Creates a new <see cref="ProjectFontDefinition" /> with a different legacy scale.
    /// </summary>
    /// <param name="scale">The legacy font scale.</param>
    /// <returns>A modified <see cref="ProjectFontDefinition" />.</returns>
    public ProjectFontDefinition WithLegacyFontScale(float scale) => new(this.SpriteSheetId, this.FontId, this.LegacyFontId, scale);
}