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
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var shadowOffset = lineThickness * GameSettings.Instance.InversePixelsPerUnit;
                var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

                var viewRatio = GameSettings.Instance.GetPixelAgnosticRatio(viewBoundingArea.Height, this.Entity.Scene.Game.ViewportSize.Y);
                var scale = new Vector2(viewRatio * 0.5f);
                var offset = scale.X * GizmoPointSize * GameSettings.Instance.InversePixelsPerUnit * 0.5f; // The extra 0.5f is to center it
                
                spriteBatch.Draw(this._neutralAxisTriangleSprite, this.NeutralAxisPosition - new Vector2(offset) + shadowOffsetVector, scale, this.EditorService.DropShadowColor);
                spriteBatch.Draw(this._xAxisArrowSprite, this.XAxisPosition - new Vector2(offset) + shadowOffsetVector, scale, this.EditorService.DropShadowColor);
                spriteBatch.Draw(this._yAxisArrowSprite, this.YAxisPosition - new Vector2(offset) + shadowOffsetVector, scale, this.EditorService.DropShadowColor);
                
                base.Render(frameTime, viewBoundingArea);
                
                spriteBatch.Draw(this._xAxisArrowSprite, this.XAxisPosition - new Vector2(offset), scale, this.EditorService.XAxisColor);
                spriteBatch.Draw(this._yAxisArrowSprite, this.YAxisPosition - new Vector2(offset), scale, this.EditorService.YAxisColor);
                spriteBatch.Draw(this._neutralAxisTriangleSprite, this.NeutralAxisPosition + new Vector2(offset), -scale, this.EditorService.XAxisColor);
                spriteBatch.Draw(this._neutralAxisTriangleSprite, this.NeutralAxisPosition - new Vector2(offset), scale, this.EditorService.YAxisColor);
            }
        }

        /// <inheritdoc />
        protected override bool ShouldBeEnabled() {
            return this.SelectionService.SelectedEntity != null && this.EditorService.SelectedGizmo == GizmoKind.Translation;
        }
    }
}