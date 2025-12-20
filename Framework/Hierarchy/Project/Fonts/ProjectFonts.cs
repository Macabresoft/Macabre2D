namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Newtonsoft.Json;

/// <summary>
/// Fonts associated with the project.
/// </summary>
[DataContract]
public class ProjectFonts {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<ProjectFontKey, ProjectFontDefinition> _categoryAndCultureToFontDefinition = [];

    /// <summary>
    /// Removes the font with the given <see cref="FontCategory" /> and <see cref="ResourceCulture" />.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>A value indicating whether the font was removed.</returns>
    public bool RemoveFont(FontCategory category, ResourceCulture culture) => this.RemoveFont(new ProjectFontKey(category, culture));

    /// <summary>
    /// Removes the font with the given <see cref="ProjectFontKey" />.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>A value indicating whether the font was removed.</returns>
    public bool RemoveFont(ProjectFontKey key) => this._categoryAndCultureToFontDefinition.Remove(key);

    /// <summary>
    /// Sets a font for the given <see cref="FontCategory" /> and <see cref="ResourceCulture" />.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="fontId">The font identifier.</param>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    public void SetFont(FontCategory category, ResourceCulture culture, Guid fontId, Guid spriteSheetId) => this.SetFont(new ProjectFontKey(category, culture), fontId, spriteSheetId);

    /// <summary>
    /// Sets a font for the given <see cref="ProjectFontKey" />.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="fontId">The font identifier.</param>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    public void SetFont(ProjectFontKey key, Guid fontId, Guid spriteSheetId) => this.SetFont(key, new ProjectFontDefinition(fontId, spriteSheetId));

    /// <summary>
    /// Sets a font for the given <see cref="ProjectFontKey" />.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="fontDefinition">The font definition.</param>
    public void SetFont(ProjectFontKey key, ProjectFontDefinition fontDefinition) {
        if (key.Category == FontCategory.None) {
            return;
        }

        this._categoryAndCultureToFontDefinition[key] = fontDefinition;
    }

    /// <summary>
    /// Tries to get the font identifier for the given <see cref="FontCategory" /> and <see cref="ResourceCulture" />.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="fontDefinition">The font definition.</param>
    /// <returns>A value indicating whether the font was found.</returns>
    public bool TryGetFont(FontCategory category, ResourceCulture culture, out ProjectFontDefinition fontDefinition) => this.TryGetFont(new ProjectFontKey(category, culture), out fontDefinition);

    /// <summary>
    /// Tries to get the font identifier for the given <see cref="ProjectFontKey" />.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="fontDefinition">The font definition.</param>
    /// <returns>A value indicating whether the font was found.</returns>
    public bool TryGetFont(ProjectFontKey key, out ProjectFontDefinition fontDefinition) => this._categoryAndCultureToFontDefinition.TryGetValue(key, out fontDefinition);

    /// <summary>
    /// Tries to set the font for the given <see cref="ProjectFontKey" />, unless the sprite sheet is null on the <see cref="SpriteSheetFont" />.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="font">The font.</param>
    /// <returns>A value indicating whether the font was set.</returns>
    public bool TrySetFont(ProjectFontKey key, SpriteSheetFont font) {
        if (font.SpriteSheet == null || key.Category == FontCategory.None) {
            return false;
        }

        this.SetFont(key, font.Id, font.SpriteSheet.ContentId);
        return true;
    }
}