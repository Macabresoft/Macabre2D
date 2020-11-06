namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Graphics;
    using System.Linq;

    /// <summary>
    /// The default rendering system, which attempts to render every <see
    /// cref="IGameRenderableComponent" /> in the scene.
    /// </summary>
    public class RenderSystem : GameSystem {
        private readonly QuadTree<IGameRenderableComponent> _renderTree = new QuadTree<IGameRenderableComponent>(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        /// <inheritdoc />
        public override SystemLoop Loop => SystemLoop.Render;

        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                this._renderTree.Clear();

                foreach (var component in this.Scene.RenderableComponents) {
                    this._renderTree.Insert(component);
                }

                foreach (var camera in this.Scene.CameraComponents) {
                    var potentialRenderables = 
                        this._renderTree
                            .RetrievePotentialCollisions(camera.BoundingArea)
                            .Where(x => (x.Entity.Layers & camera.LayersToRender) != Layers.None)
                            .ToList();

                    if (potentialRenderables.Any()) {
                        camera.Render(frameTime, spriteBatch, potentialRenderables);
                    }
                }
            }
        }
    }
}