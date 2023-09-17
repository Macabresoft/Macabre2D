namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A <see cref="IDisplayNameHandler" /> for <see cref="Layers" />.
/// </summary>
public class LayersDisplayNameHandler : IDisplayNameHandler {
    private readonly IProjectService _projectService;

    public LayersDisplayNameHandler(IProjectService projectService) {
        this._projectService = projectService;
    }

    /// <inheritdoc />
    public string GetDisplayName(object value) {
        var displayName = string.Empty;
        if (value is Layers layers) {
            foreach (var enumValue in Enum.GetValues<Layers>().Where(x => layers.HasFlag(x))) {
                var enumName = this._projectService.CurrentProject.LayerSettings.GetName(enumValue);
                displayName = string.IsNullOrEmpty(displayName) ? enumName : $"{displayName}, {enumName}";
            }
        }

        return displayName;
    }
}