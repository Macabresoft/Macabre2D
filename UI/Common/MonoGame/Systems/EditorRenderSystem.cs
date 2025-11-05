namespace Macabresoft.Macabre2D.UI.Common;

using System.Linq;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A render system built explicitly for the <see cref="IEditorGame" />.
/// </summary>
public class EditorRenderSystem : RenderSystem {
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
    public override void Render(FrameTime frameTime) {
        if (this.Game is IEditorGame { SpriteBatch: { } spriteBatch, Camera: { } camera } sceneEditor) {
            var scene = this._sceneService.CurrentScene ?? this._sceneService.CurrentlyEditing?.Scene ?? EmptyObject.Scene;
            this.Scene.BackgroundColor = scene.BackgroundColor;
            spriteBatch.GraphicsDevice.Clear(scene.BackgroundColor);
            this._renderTree.Clear();

            foreach (var component in sceneEditor.CurrentScene.RenderableEntities.Where(x => x is EditorGrid)) {
                this._renderTree.Insert(component);
            }
            
            foreach (var component in scene.RenderableEntities) {
                this._renderTree.Insert(component);
            }
            
            foreach (var component in sceneEditor.CurrentScene.RenderableEntities.Where(x => !(x is EditorGrid))) {
                this._renderTree.Insert(component);
            }

            camera.Render(frameTime, spriteBatch, this._renderTree);
        }
    }
}