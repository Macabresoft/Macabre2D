namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// The base project node in the project tree.
/// </summary>
public class ProjectNode {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectNode" /> class.
    /// </summary>
    /// <param name="rootContentDirectory">The root content directory.</param>
    /// <param name="project">The project.</param>
    public ProjectNode(IContentDirectory rootContentDirectory, IGameProject project) {
        this.Children = [
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