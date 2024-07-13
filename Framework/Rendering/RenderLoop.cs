namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Linq;

/// <summary>
/// The default rendering loop, which attempts to render every
/// <see cref="IRenderableEntity" /> in the scene.
/// </summary>
public class RenderLoop : Loop {
    private QuadTree<IRenderableEntity> _renderTree = QuadTree<IRenderableEntity>.Default;

    /// <inheritdoc />
    public override LoopKind Kind => LoopKind.Render;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.Scene.PropertyChanged -= this.Scene_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);

        if (!Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.Scene.PropertyChanged += this.Scene_PropertyChanged;
        }

        this.ResetTree();
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        this.InsertRenderables();

        foreach (var camera in this.Scene.Cameras) {
            camera.Render(frameTime, this.Game.SpriteBatch, this._renderTree);
        }
    }

    private void InsertRenderables() {
        this._renderTree.Clear();

        if (!this.Scene.BoundingArea.IsEmpty) {
            foreach (var entity in this.Scene.RenderableEntities.Where(x => x.RenderOutOfBounds || x.BoundingArea.OverlapsExclusive(this.Scene.BoundingArea))) {
                this._renderTree.Insert(entity);
            }
        }
        else {
            foreach (var entity in this.Scene.RenderableEntities) {
                this._renderTree.Insert(entity);
            }
        }
    }

    private void ResetTree() {
        this._renderTree.Clear();

        if (this.Scene.BoundingArea.IsEmpty) {
            this._renderTree = QuadTree<IRenderableEntity>.Default;
        }
        else {
            this._renderTree = new QuadTree<IRenderableEntity>(
                0,
                this.Scene.BoundingArea.Minimum.X,
                this.Scene.BoundingArea.Minimum.Y,
                this.Scene.BoundingArea.Width,
                this.Scene.BoundingArea.Height);
        }
    }

    private void Scene_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IScene.BoundingArea)) {
            this.ResetTree();
        }
    }
}