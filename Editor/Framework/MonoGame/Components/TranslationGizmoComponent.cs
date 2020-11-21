namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Avalonia.Input;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MouseButton = Macabresoft.Macabre2D.Framework.MouseButton;

    /// <summary>
    /// A gizmo/component that allows the user to translate entities in the editor.
    /// </summary>
    public class TranslationGizmoComponent : BaseAxisGizmoComponent {
        private Sprite _neutralAxisTriangleSprite;
        private Vector2 _unmovedPosition;
        private Sprite _xAxisArrowSprite;
        private Sprite _yAxisArrowSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationGizmoComponent" /> class.
        /// </summary>
        public TranslationGizmoComponent(IEditorService editorService, IEntitySelectionService selectionService) : base(editorService, selectionService) {
        }

        /// <inheritdoc />
        public override GizmoKind GizmoKind => GizmoKind.Translation;

        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            if (this.Entity.Scene.Game.GraphicsDevice is GraphicsDevice graphicsDevice) {
                this._xAxisArrowSprite = PrimitiveDrawer.CreateForwardArrowSprite(graphicsDevice, GizmoPointSize);
                this._yAxisArrowSprite = PrimitiveDrawer.CreateUpwardsArrowSprite(graphicsDevice, GizmoPointSize);
                this._neutralAxisTriangleSprite = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(graphicsDevice, new Point(GizmoPointSize));
            }
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var shadowOffset = lineThickness * GameSettings.Instance.InversePixelsPerUnit;
                var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

                var viewRatio = GameSettings.Instance.GetPixelAgnosticRatio(viewBoundingArea.Height, this.Entity.Scene.Game.ViewportSize.Y);
                var scale = new Vector2(viewRatio);
                var offset = viewRatio * GizmoPointSize * GameSettings.Instance.InversePixelsPerUnit * 0.5f; // The extra 0.5f is to center it

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
        public override bool Update(FrameTime frameTime, InputState inputState) {
            var result = base.Update(frameTime, inputState);

            var mousePosition = this.Camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);

            if (inputState.IsButtonNewlyPressed(MouseButton.Left)) {
                var viewRatio = GameSettings.Instance.GetPixelAgnosticRatio(this.Camera.ViewHeight, this.Entity.Scene.Game.ViewportSize.Y);
                var radius = viewRatio * GizmoPointSize * GameSettings.Instance.InversePixelsPerUnit * 0.5f;
                result = true;
                if (Vector2.Distance(this.XAxisPosition, mousePosition) < radius) {
                    this.StartDrag(GizmoAxis.X);
                }
                else if (Vector2.Distance(this.YAxisPosition, mousePosition) < radius) {
                    this.StartDrag(GizmoAxis.Y);
                }
                else if (Vector2.Distance(this.NeutralAxisPosition, mousePosition) < radius) {
                    this.StartDrag(GizmoAxis.Neutral);
                }
                else {
                    result = false;
                }
            }
            else if (this.CurrentAxis != GizmoAxis.None) {
                if (inputState.IsButtonHeld(MouseButton.Left)) {
                    // TODO: handle snapped positions
                    var newPosition = mousePosition;
                    if (this.CurrentAxis == GizmoAxis.X) {
                        newPosition = this.MoveAlongAxis(this.NeutralAxisPosition, this.XAxisPosition, mousePosition) - (this.XAxisPosition - this.NeutralAxisPosition);
                    }
                    else if (this.CurrentAxis == GizmoAxis.Y) {
                        newPosition = this.MoveAlongAxis(this.NeutralAxisPosition, this.YAxisPosition, mousePosition) - (this.YAxisPosition - this.NeutralAxisPosition);
                    }

                    this.UpdatePosition(newPosition);
                    result = true;
                }
                else {
                    // TODO: add to undo service
                    this.CurrentAxis = GizmoAxis.None;
                    this.SetCursor(StandardCursorType.None);
                }
            }

            return result;
        }

        /// <inheritdoc />
        protected override bool ShouldBeEnabled() {
            return this.SelectionService.SelectedEntity != null && this.EditorService.SelectedGizmo == GizmoKind.Translation;
        }

        private void StartDrag(GizmoAxis axis) {
            this._unmovedPosition = this.NeutralAxisPosition;
            this.CurrentAxis = axis;
            this.SetCursor(StandardCursorType.DragMove);
        }

        private void UpdatePosition(Vector2 newPosition) {
            this.SelectionService.SelectedEntity?.SetWorldPosition(newPosition);
        }
    }
}