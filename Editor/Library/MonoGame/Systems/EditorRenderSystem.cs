namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Systems {
    using System.Linq;
    using Framework;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame.Components;
    using Microsoft.Xna.Framework.Graphics;
    using Services;

    /// <summary>
    /// A render system built explicitly for the <see cref="ISceneEditor" />.
    /// </summary>
    internal class EditorRenderSystem : GameSystem {
        private readonly QuadTree<IGameRenderableComponent> _renderTree = new(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        private readonly IProjectService _projectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorRenderSystem" /> class.
        /// </summary>
        /// <param name="projectService">The project service.</param>
        public EditorRenderSystem(IProjectService projectService) {
            this._projectService = projectService;
        }

        /// <inheritdoc />
        public override SystemLoop Loop => SystemLoop.Render;

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this.Scene.Game is ISceneEditor { SpriteBatch: SpriteBatch spriteBatch, Camera: ICameraComponent camera } sceneEditor) {
                this._renderTree.Clear();

                foreach (var component in sceneEditor.Scene.RenderableComponents.Where(x => x is EditorGridComponent)) {
                    this._renderTree.Insert(component);
                }
                
                foreach (var component in this._projectService.CurrentScene.RenderableComponents) {
                    this._renderTree.Insert(component);
                }
                
                foreach (var component in sceneEditor.Scene.RenderableComponents.Where(x => !(x is EditorGridComponent))) {
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