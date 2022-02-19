namespace Macabresoft.Macabre2D.UI.Common;

using System.Linq;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A render system built explicitly for the <see cref="IEditorGame" />.
/// </summary>
public class EditorRenderSystem : LoopSystem {
    private readonly QuadTree<IRenderableEntity> _renderTree = new(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

    private readonly ISceneService _sceneService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorRenderSystem" /> class.
    /// </summary>
    /// <param name="sceneService">The scene service.</param>
    public EditorRenderSystem(ISceneService sceneService) {
        this._sceneService = sceneService;
    }

    /// <inheritdoc />
    public override SystemKind Kind => SystemKind.Render;

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.Scene.Game is IEditorGame { SpriteBatch: SpriteBatch spriteBatch, Camera: ICamera camera } sceneEditor) {
            this.Scene.BackgroundColor = this._sceneService.CurrentScene.BackgroundColor;
            spriteBatch.GraphicsDevice.Clear(this._sceneService.CurrentScene.BackgroundColor);
            this._renderTree.Clear();

            foreach (var component in sceneEditor.Scene.RenderableEntities.Where(x => x is EditorGrid)) {
                this._renderTree.Insert(component);
            }

            foreach (var component in this._sceneService.CurrentScene.RenderableEntities) {
                this._renderTree.Insert(component);
            }

            foreach (var component in sceneEditor.Scene.RenderableEntities.Where(x => !(x is EditorGrid))) {
                this._renderTree.Insert(component);
            }

            var potentialRenderables = this._renderTree.RetrievePotentialCollisions(camera.BoundingArea);
            if (potentialRenderables.Any()) {
                camera.Render(frameTime, spriteBatch, potentialRenderables);
            }
        }
    }
}