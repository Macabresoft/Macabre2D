namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A gizmo/component that allows the user to translate entities in the editor.
    /// </summary>
    public class TranslationGizmoComponent : BaseAxisGizmoComponent {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationGizmoComponent" /> class.
        /// </summary>
        public TranslationGizmoComponent(IEditorService editorService, IEntitySelectionService selectionService) : base(editorService, selectionService) {
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            base.Render(frameTime, viewBoundingArea);
        }

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
        }


        /// <inheritdoc />
        protected override bool ShouldBeEnabled() {
            return this.SelectionService.SelectedEntity != null && this.EditorService.SelectedGizmo == GizmoKind.Translation;
        }
    }
}