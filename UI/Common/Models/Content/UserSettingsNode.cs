namespace Macabre2D.UI.Common;

using System.Collections.Generic;
using Macabre2D.Framework;

/// <summary>
/// A node of user settings.
/// </summary>
public class UserSettingsNode {

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsNode"/> class.
    /// </summary>
    /// <param name="settings">The user settings.</param>
    public UserSettingsNode(UserSettings settings) {
        this.Children = [
            settings.Audio,
            settings.Display,
            settings.Gameplay,
            settings.Input,
            settings.Rendering
        ];
    }
    
    /// <summary>
    /// Gets the children.
    /// </summary>
    public IReadOnlyCollection<object> Children { get; }
}