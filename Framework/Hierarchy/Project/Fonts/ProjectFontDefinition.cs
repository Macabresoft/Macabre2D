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
    /// The MonoGame font identifier which references a <see cref="SpriteFont" />.
    /// </summary>
    [DataMember]
    public readonly Guid MonoGameFontId;

    /// <summary>
    /// The sprite sheet identifier which references a <see cref="SpriteSheetFont" />.
    /// </summary>
    [DataMember]
    public readonly Guid SpriteSheetId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontDefinition" /> class.
    /// </summary>
    public ProjectFontDefinition() : this(Guid.Empty, Guid.Empty, Guid.Empty) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontDefinition" /> class.
    /// </summary>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    /// <param name="fontId">The font identifier.</param>
    /// <param name="monoGameFontId">The mono game font identifier.</param>
    public ProjectFontDefinition(Guid spriteSheetId, Guid fontId, Guid monoGameFontId) {
        this.SpriteSheetId = spriteSheetId;
        this.FontId = fontId;
        this.MonoGameFontId = monoGameFontId;
    }

    /// <summary>
    /// Creates a new <see cref="ProjectFontDefinition" /> with a different MonoGame font, while maintaining the original sprite sheet font.
    /// </summary>
    /// <param name="monoGameFontId">The MonoGame font identifier.</param>
    /// <returns>A modified <see cref="ProjectFontDefinition" />.</returns>
    public ProjectFontDefinition WithMonoGameFont(Guid monoGameFontId) => new(this.SpriteSheetId, this.FontId, monoGameFontId);

    /// <summary>
    /// Creates a new <see cref="ProjectFontDefinition" /> with a different sprite sheet font, while maintaining the original MonoGame font.
    /// </summary>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    /// <param name="fontId">The font identifier.</param>
    /// <returns>A modified <see cref="ProjectFontDefinition" />.</returns>
    public ProjectFontDefinition WithSpriteSheetFont(Guid spriteSheetId, Guid fontId) => new(spriteSheetId, fontId, this.MonoGameFontId);
}