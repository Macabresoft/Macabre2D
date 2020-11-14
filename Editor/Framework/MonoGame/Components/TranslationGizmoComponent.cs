namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A gizmo/component that allows the user to translate entities in the editor.
    /// </summary>
    public class TranslationGizmoComponent : GameUpdateableComponent {
        private readonly IEditorService _editorService;
        private readonly IEntitySelectionService _selectionService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationGizmoComponent" /> class.
        /// </summary>
        public TranslationGizmoComponent(IEditorService editorService, IEntitySelectionService selectionService) {
            this._editorService = editorService;
            this._selectionService = selectionService;
        }
        
        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            throw new System.NotImplementedException();
        }
    }
}