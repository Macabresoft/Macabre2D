namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Macabresoft.AvaloniaEx;

/// <summary>
/// A node for <see cref="ProjectFonts"/> in the project tree.
/// </summary>
public class ProjectFontsNode {

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontsNode"/> class.
    /// </summary>
    /// <param name="fonts">The project fonts.</param>
    /// <param name="undoService">The undo service.</param>
    public ProjectFontsNode(ProjectFonts fonts, IUndoService undoService) {
        this.Children = Enum.GetValues<ResourceCulture>()
            .Select(culture => new ProjectFontModelCollection(fonts, culture, undoService))
            .ToList();
    }
    
    /// <summary>
    /// Gets the children.
    /// </summary>
    public IReadOnlyCollection<ProjectFontModelCollection> Children { get; }
}