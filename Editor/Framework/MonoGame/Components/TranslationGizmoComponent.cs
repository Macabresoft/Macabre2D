namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A gizmo/component that allows the user to translate entities in the editor.
    /// </summary>
    public class TranslationGizmoComponent : BaseAxisGizmoComponent {

        private Sprite _xAxisArrowSprite;
        private Sprite _xNeutralTriangleSprite;
        private Sprite _yAxisArrowSprite;
        private Sprite _yNeutralTriangleSprite;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationGizmoComponent" /> class.
        /// </summary>
        public TranslationGizmoComponent(IEditorService editorService, IEntitySelectionService selectionService) : base(editorService, selectionService) {
        }
        
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            // TODO: place the camera in the EditorService and actually make a unique entity for each gizmo so they can use physics bodies and sprite renderers (NO CUSTOM LOGIC)
            //this._xAxisArrowSprite = 
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