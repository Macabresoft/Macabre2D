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
    private readonly Dictionary<ProjectFontKey, ProjectFontDefinition> _categoryAndCultureToFontDefinition = new();

    /// <summary>
    /// Sets a font for the given <see cref="FontCategory" /> and <see cref="ResourceCulture" />.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="fontId">The font identifier.</param>
    /// <param name="spriteSheetId">The sprite sheet identifier.</param>
    public void SetFont(FontCategory category, ResourceCulture culture, Guid fontId, Guid spriteSheetId) {
        if (category == FontCategory.None) {
            return;
        }

        this._categoryAndCultureToFontDefinition[new ProjectFontKey(category, culture)] = new ProjectFontDefinition(fontId, spriteSheetId);
    }

    /// <summary>
    /// Tries to get the font identifier for the given <see cref="FontCategory" /> amd <see cref="ResourceCulture" />.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="fontDefinition">The font definition.</param>
    /// <returns>A value indicating whether the font was found.</returns>
    public bool TryGetFont(FontCategory category, ResourceCulture culture, out ProjectFontDefinition fontDefinition) => this._categoryAndCultureToFontDefinition.TryGetValue(new ProjectFontKey(category, culture), out fontDefinition);
}