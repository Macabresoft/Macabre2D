namespace Macabresoft.Macabre2D.UI.Common.MonoGame.Systems {
    using Macabresoft.Macabre2D.UI.Common.MonoGame.Entities;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// An update system built explicitly for the <see cref="ISceneEditor" />.
    /// </summary>
    public class EditorUpdateSystem : UpdateSystem {
        private readonly IEditorService _editorService;
        private readonly IGizmo _selectorGizmo;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorUpdateSystem" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="selectorGizmo">The selector gizmo.</param>
        public EditorUpdateSystem(IEditorService editorService, IGizmo selectorGizmo) {
            this._editorService = editorService;
            this._selectorGizmo = selectorGizmo;
        }

        /// <inheritdoc />
        public override SystemLoop Loop => SystemLoop.Update;

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this.Scene.Game is ISceneEditor sceneEditor) {
                var performedActions = false;

                if (sceneEditor.SelectedGizmo is IGizmo gizmo) {
                    performedActions = gizmo.Update(frameTime, inputState);
                }

                if (!performedActions) {
                    this._selectorGizmo.Update(frameTime, inputState);
                }
            }

            base.Update(frameTime, inputState);
        }
    }
}