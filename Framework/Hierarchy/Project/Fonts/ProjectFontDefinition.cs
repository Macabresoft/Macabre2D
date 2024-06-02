namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;

/// <summary>
/// A definition for a project font that includes the sprite sheet and font identifier.
/// </summary>
[DataContract]
public readonly struct ProjectFontDefinition {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontDefinition" /> class.
    /// </summary>
    public ProjectFontDefinition() : this(Guid.Empty, Guid.Empty) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontDefinition" /> class.
    /// </summary>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    /// <param name="fontId">The font identifier.</param>
    public ProjectFontDefinition(Guid spriteSheetId, Guid fontId) {
        this.SpriteSheetId = spriteSheetId;
        this.FontId = fontId;
    }

    /// <summary>
    /// The sprite sheet identifier.
    /// </summary>
    [DataMember]
    public readonly Guid SpriteSheetId;

    /// <summary>
    /// The font identifier.
    /// </summary>
    [DataMember]
    public readonly Guid FontId;
}