namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Graphics;
    using System.Linq;

    /// <summary>
    /// A service which does a sorted render loop over visible renderable components.
    /// </summary>
    public sealed class RenderService : GameService {
        private readonly QuadTree<IGameRenderableComponent> _renderTree = new QuadTree<IGameRenderableComponent>(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this.Scene != null) {
                this._renderTree.Clear();

                foreach (var component in this.Scene.RenderableComponents) {
                    this._renderTree.Insert(component);
                }

                foreach (var camera in this.Scene.CameraComponents) {
                    var potentialRenderables = this._renderTree.RetrievePotentialCollisions(camera.BoundingArea);

                    if (potentialRenderables.Any()) {
                        this.Scene.GameLoop.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, camera.SamplerState, null, RasterizerState.CullNone, camera.Shader?.Effect, camera.ViewMatrix);

                        foreach (var component in potentialRenderables) {
                            // As long as it doesn't equal Layers.None, at least one of the layers
                            // defined on the component are also to be rendered by LayersToRender.
                            if ((component.Entity.Layers & camera.LayersToRender) != Layers.None) {
                                component.Render(frameTime, camera.BoundingArea);
                            }
                        }

                        this.Scene.GameLoop.SpriteBatch.SpriteBatch.End();
                    }
                }
            }
        }
    }
}