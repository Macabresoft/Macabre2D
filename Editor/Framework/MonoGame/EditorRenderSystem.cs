namespace Macabresoft.Macabre2D.Editor.Library.MonoGame {
    using System.Linq;
    using Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Services;

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

            if (this.Scene.Game is ISceneEditor sceneEditor && sceneEditor.SpriteBatch is SpriteBatch spriteBatch && sceneEditor.Camera is ICameraComponent camera) {
                this._renderTree.Clear();

                foreach (var component in sceneEditor.Scene.RenderableComponents) {
                    this._renderTree.Insert(component);
                }
                
                foreach (var component in this._sceneService.CurrentScene.RenderableComponents) {
                    this._renderTree.Insert(component);
                }
                
                var potentialRenderables = this._renderTree.RetrievePotentialCollisions(camera.BoundingArea);
                if (potentialRenderables.Any()) {
                    camera.Render(frameTime, spriteBatch, potentialRenderables);
                }
            }
        }
    }
}