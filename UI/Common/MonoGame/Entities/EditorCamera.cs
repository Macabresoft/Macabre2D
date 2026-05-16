namespace Macabre2D.UI.Common;

using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Camera for the editor.
/// </summary>
public class EditorCamera : Camera {

    /// <inheritdoc />
    public override void RenderInScreenSpace(FrameTime frameTime, SpriteBatch spriteBatch, IReadonlyQuadTree<IScreenSpaceRenderer> renderTree) {
        this.RenderInScreenSpace(frameTime, spriteBatch, renderTree, this.BoundingArea, this.GetViewMatrix(), this.LayersToRender, this.LayersToExcludeFromRender);
    }

    /// <inheritdoc />
    protected override Vector2 CreateSize() => new(this.Game.ViewportSize.X, this.Game.ViewportSize.Y);
}