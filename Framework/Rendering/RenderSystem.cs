namespace Macabresoft.Macabre2D.Framework;

using System.Linq;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// The default rendering system, which attempts to render every
/// <see cref="IRenderableEntity" /> in the scene.
/// </summary>
public class RenderSystem : UpdateableSystem {
    private readonly QuadTree<IRenderableEntity> _renderTree = new(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

    /// <inheritdoc />
    public override SystemLoop Loop => SystemLoop.Render;

    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
            this._renderTree.Clear();

            foreach (var entity in this.Scene.RenderableEntities) {
                this._renderTree.Insert(entity);
            }

            foreach (var camera in this.Scene.Cameras) {
                var potentialRenderables =
                    this._renderTree
                        .RetrievePotentialCollisions(camera.BoundingArea)
                        .Where(x => (x.Layers & camera.LayersToRender) != Layers.None)
                        .ToList();

                if (potentialRenderables.Any()) {
                    camera.Render(frameTime, spriteBatch, potentialRenderables);
                }
            }
        }
    }
}