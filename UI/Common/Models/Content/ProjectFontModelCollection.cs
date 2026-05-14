namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Macabresoft.AvaloniaEx;

/// <summary>
/// A collection of <see cref="ProjectFontModel" /> to display in the project tree.
/// </summary>
public class ProjectFontModelCollection {

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontModelCollection" /> class.
    /// </summary>
    /// <param name="fonts">The fonts.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="undoService">The undo service.</param>
    public ProjectFontModelCollection(ProjectFonts fonts, ResourceCulture culture, IUndoService undoService) {
        this.Culture = culture;

        var categories = Enum.GetValues<FontCategory>().ToList();
        categories.Remove(FontCategory.None);

        this.Models = categories
            .Select(category => new ProjectFontModel(new ProjectFontKey(category, this.Culture), undoService, fonts))
            .ToList();
    }

    /// <summary>
    /// Gets the culture.
    /// </summary>
    public ResourceCulture Culture { get; }

    /// <summary>
    /// Gets the font models assigned to the current culture.
    /// </summary>
    public IReadOnlyCollection<ProjectFontModel> Models { get; }
}