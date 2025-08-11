namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Camera for the editor.
/// </summary>
public class EditorCamera : Camera {
    /// <inheritdoc />
    protected override Vector2 CreateSize() => new(this.Game.ViewportSize.X, this.Game.ViewportSize.Y);
}