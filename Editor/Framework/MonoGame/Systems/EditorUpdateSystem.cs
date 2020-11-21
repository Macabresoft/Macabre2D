namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Systems {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// An update system built explicitly for the <see cref="ISceneEditor" />.
    /// </summary>
    public class EditorUpdateSystem : UpdateSystem {
        private readonly IEditorService _editorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorUpdateSystem" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        public EditorUpdateSystem(IEditorService editorService) {
            this._editorService = editorService;
        }

        /// <inheritdoc />
        public override SystemLoop Loop => SystemLoop.Update;

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            if (this.Scene.Game is ISceneEditor sceneEditor) {
                var performedActions = false;

                if (this._editorService.SelectedGizmo == GizmoKind.Translation) {
                    performedActions = sceneEditor.TranslationGizmo.Update(frameTime, inputState);
                }
                else if (this._editorService.SelectedGizmo == GizmoKind.Scale) {
                    // TODO: scale
                }

                if (!performedActions) {
                    sceneEditor.SelectorGizmo.Update(frameTime, inputState);
                }
            }

            base.Update(frameTime, inputState);
        }
    }
}