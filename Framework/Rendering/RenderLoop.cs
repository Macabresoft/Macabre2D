namespace Macabresoft.Macabre2D.Framework;

using System.Linq;

/// <summary>
/// The default rendering loop, which attempts to render every
/// <see cref="IRenderableEntity" /> in the scene.
/// </summary>
public class RenderLoop : Loop {
    private readonly QuadTree<IRenderableEntity> _renderTree = new(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

    /// <inheritdoc />
    public override LoopKind Kind => LoopKind.Render;

    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.Game is { } game) {
            var enabledLayers = game.Project.Settings.LayerSettings.EnabledLayers;
            this._renderTree.Clear();

            foreach (var entity in this.Scene.RenderableEntities) {
                this._renderTree.Insert(entity);
            }

            foreach (var camera in this.Scene.Cameras) {
                var potentialRenderables =
                    this._renderTree
                        .RetrievePotentialCollisions(camera.BoundingArea)
                        .Where(x => (x.Layers & camera.LayersToRender & enabledLayers) != Layers.None)
                        .ToList();

                if (potentialRenderables.Any()) {
                    camera.Render(frameTime, game.SpriteBatch, potentialRenderables);
                }
            }
        }
    }
}