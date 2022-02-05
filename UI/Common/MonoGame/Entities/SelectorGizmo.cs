namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using Avalonia.Threading;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A component which selects entities and components based on their bounding areas.
/// </summary>
public class SelectorGizmo : Entity, IGizmo {
    private readonly ISceneService _sceneService;
    private ICamera _camera;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectorGizmo" /> class.
    /// </summary>
    /// <param name="sceneService">The scene service.</param>
    public SelectorGizmo(ISceneService sceneService) : base() {
        this._sceneService = sceneService;
    }

    /// <inheritdoc />
    public GizmoKind GizmoKind => GizmoKind.Selector;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        if (!this.TryGetParentEntity(out this._camera)) {
            throw new NotSupportedException("Could not find a camera ancestor.");
        }
    }

    /// <inheritdoc />
    public bool Update(FrameTime frameTime, InputState inputState) {
        var result = false;
        if (this._camera != null &&
            !Framework.Scene.IsNullOrEmpty(this._sceneService.CurrentScene) &&
            inputState.IsMouseButtonNewlyReleased(MouseButton.Left)) {
            IEntity selected = null;
            var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);
            var potentials = this._sceneService.CurrentScene.RenderableEntities.Where(x => x.BoundingArea.Contains(mousePosition));

            foreach (var potential in potentials) {
                if (potential is ITileableEntity tileableEntity) {
                    if (tileableEntity.HasActiveTileAt(mousePosition)) {
                        selected = potential;
                        break;
                    }
                }
                else {
                    selected = potential;
                    break;
                }
            }

            if (this._sceneService.Selected != selected) {
                if (selected == null && this._sceneService.IsEntityContext) {
                    Dispatcher.UIThread.Post(() => this._sceneService.Selected = null);
                }
                else {
                    this.CheckForPrefab(selected, out selected);
                    Dispatcher.UIThread.Post(() => this._sceneService.Selected = selected);
                }
            }

            result = true;
        }

        return result;
    }

    private bool CheckForPrefab(IEntity selected, out IEntity finalSelected) {
        finalSelected = selected;
        var result = false;
        var potential = selected;

        while (potential.Parent != Empty && potential.Parent != this._sceneService.CurrentScene) {
            potential = potential.Parent;

            if (potential is PrefabContainer && !this.CheckForPrefab(potential, out finalSelected)) {
                result = true;
                break;
            }
        }

        return result;
    }
}