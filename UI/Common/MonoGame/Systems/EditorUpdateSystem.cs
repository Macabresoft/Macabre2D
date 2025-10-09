namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Threading;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// An update system built explicitly for the <see cref="IEditorGame" />.
/// </summary>
public class EditorUpdateSystem : UpdateSystem {
    private readonly IEntityService _entityService;
    private readonly ISceneService _sceneService;
    private readonly IGizmo _selectorGizmo;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorUpdateSystem" /> class.
    /// </summary>
    /// <param name="entityService">The entity service.</param>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="selectorGizmo">The selector gizmo.</param>
    public EditorUpdateSystem(IEntityService entityService, ISceneService sceneService, IGizmo selectorGizmo) {
        this._entityService = entityService;
        this._sceneService = sceneService;
        this._selectorGizmo = selectorGizmo;
    }

    /// <inheritdoc />
    public override UpdateSystemKind Kind => UpdateSystemKind.Update;

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.Game is IEditorGame sceneEditor) {
            var performedActions = false;

            if (inputState.IsKeyNewlyReleased(Keys.Escape)) {
                if (this._sceneService.Selected is IEntity and not IScene) {
                    Dispatcher.UIThread.Post(() => this._sceneService.Selected = null);
                    performedActions = true;
                }
            }

            if (!performedActions && this._entityService.Selected != null && sceneEditor.SelectedGizmo is { } gizmo) {
                performedActions = gizmo.Update(frameTime, inputState);
            }

            if (!performedActions) {
                this._selectorGizmo.Update(frameTime, inputState);
            }
        }

        base.Update(frameTime, inputState);
    }
}