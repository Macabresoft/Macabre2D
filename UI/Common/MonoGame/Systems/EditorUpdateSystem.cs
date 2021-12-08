namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Macabre2D.Framework;

/// <summary>
/// An update system built explicitly for the <see cref="IEditorGame" />.
/// </summary>
public class EditorUpdateSystem : UpdateSystem {
    private readonly IEntityService _entityService;
    private readonly IGizmo _selectorGizmo;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorUpdateSystem" /> class.
    /// </summary>
    /// <param name="entityService">The entity service.</param>
    /// <param name="selectorGizmo">The selector gizmo.</param>
    public EditorUpdateSystem(IEntityService entityService, IGizmo selectorGizmo) {
        this._entityService = entityService;
        this._selectorGizmo = selectorGizmo;
    }

    /// <inheritdoc />
    public override SystemLoop Loop => SystemLoop.Update;

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.Scene.Game is IEditorGame sceneEditor) {
            var performedActions = false;

            if (this._entityService.Selected != null && sceneEditor.SelectedGizmo is IGizmo gizmo) {
                performedActions = gizmo.Update(frameTime, inputState);
            }

            if (!performedActions) {
                this._selectorGizmo.Update(frameTime, inputState);
            }
        }

        base.Update(frameTime, inputState);
    }
}