namespace Macabresoft.Macabre2D.UI.Common.DisplayNameHandlers;

using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A <see cref="IDisplayNameHandler" /> for <see cref="Layers" />.
/// </summary>
public class InputActionsDisplayNameHandler  : IDisplayNameHandler {
    private readonly IProjectService _projectService;

    public InputActionsDisplayNameHandler(IProjectService projectService) {
        this._projectService = projectService;
    }

    /// <inheritdoc />
    public string GetDisplayName(object value) {
        var displayName = string.Empty;
        if (value is InputAction inputAction) {
            displayName = this._projectService.CurrentProject.Settings.InputSettings.GetName(inputAction);
        }

        return displayName;
    }
}