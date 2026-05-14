namespace Macabre2D.UI.Common;

using System.Collections.Generic;
using Macabre2D.Framework;
using Macabresoft.AvaloniaEx;

/// <summary>
/// The base project node in the project tree.
/// </summary>
public class ProjectNode {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectNode" /> class.
    /// </summary>
    /// <param name="rootContentDirectory">The root content directory.</param>
    /// <param name="project">The project.</param>
    /// <param name="undoService">The undo service.</param>
    public ProjectNode(IContentDirectory rootContentDirectory, IGameProject project, IUndoService undoService) {
        this.Children = [
            new UserSettingsNode(project.DefaultUserSettings),
            new ProjectFontsNode(project.Fonts, undoService),
            project.RenderSteps,
            project.PhysicsMaterials,
            rootContentDirectory
        ];
    }

    /// <summary>
    /// Gets the children.
    /// </summary>
    public IReadOnlyCollection<object> Children { get; }
}