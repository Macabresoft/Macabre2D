namespace Macabresoft.Macabre2D.Editor.Library.MonoGame {

    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Linq;

    /// <summary>
    /// A render system built explicitly for the <see cref="ISceneEditor" />.
    /// </summary>
    internal class EditorRenderSystem : UpdateSystem {
        private readonly QuadTree<IGameRenderableComponent> _renderTree = new QuadTree<IGameRenderableComponent>(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        private readonly ISceneService _sceneService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorRenderSystem" /> class.
        /// </summary>
        /// <param name="sceneService">The scene service.</param>
        public EditorRenderSystem(ISceneService sceneService) {
            this._sceneService = sceneService;
        }

        /// <inheritdoc />
        public override SystemLoop Loop => SystemLoop.Render;

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            base.Update(frameTime, inputState);

            if (this.Scene.Game is ISceneEditor sceneEditor && sceneEditor.SpriteBatch != null && sceneEditor.Camera != null) {
                this._renderTree.Clear();

                foreach (var component in this._sceneService.CurrentScene.RenderableComponents) {
                    this._renderTree.Insert(component);
                }

                foreach (var component in sceneEditor.Scene.RenderableComponents) {
                    this._renderTree.Insert(component);
                }

                var potentialRenderables = this._renderTree.RetrievePotentialCollisions(sceneEditor.Camera.BoundingArea);
                if (potentialRenderables.Any()) {
                    sceneEditor.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sceneEditor.Camera.SamplerState, null, RasterizerState.CullNone, sceneEditor.Camera.Shader?.Effect, sceneEditor.Camera.ViewMatrix);

                    foreach (var component in potentialRenderables) {
                        // As long as it doesn't equal Layers.None, at least one of the layers
                        // defined on the component are also to be rendered by LayersToRender.
                        if ((component.Entity.Layers & sceneEditor.Camera.LayersToRender) != Layers.None) {
                            component.Render(frameTime, sceneEditor.Camera.BoundingArea);
                        }
                    }

                    sceneEditor.SpriteBatch.End();
                }
            }
        }
    }
}