namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A gizmo/component that allows the user to translate entities in the editor.
    /// </summary>
    public class TranslationGizmoComponent : BaseAxisGizmoComponent {
        private Sprite _xAxisArrowSprite;
        private Sprite _neutralAxisTriangleSprite;
        private Sprite _yAxisArrowSprite;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationGizmoComponent" /> class.
        /// </summary>
        public TranslationGizmoComponent(IEditorService editorService, IEntitySelectionService selectionService) : base(editorService, selectionService) {
        }
        
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            if (this.Entity.Scene.Game.GraphicsDevice is GraphicsDevice graphicsDevice) {
                this._xAxisArrowSprite = PrimitiveDrawer.CreateForwardArrowSprite(graphicsDevice, GizmoPointSize);
                this._yAxisArrowSprite = PrimitiveDrawer.CreateUpwardsArrowSprite(graphicsDevice, GizmoPointSize);
                this._neutralAxisTriangleSprite = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(graphicsDevice, new Point(GizmoPointSize));
            }
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