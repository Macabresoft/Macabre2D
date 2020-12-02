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
    public sealed class TranslationGizmoComponent : BaseAxisGizmoComponent {
        private readonly IUndoService _undoService;
        private Sprite _neutralAxisTriangleSprite;
        private Vector2 _unmovedPosition;
        private Sprite _xAxisArrowSprite;
        private Sprite _yAxisArrowSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationGizmoComponent" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="selectionService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        public TranslationGizmoComponent(
            IEditorService editorService,
            ISelectionService selectionService,
            IUndoService undoService) : base(editorService, selectionService) {
            this._undoService = undoService;
        }

        /// <inheritdoc />
        public override GizmoKind GizmoKind => GizmoKind.Translation;

        /// <inheritdoc />
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
                var axis = this.GetAxisUnderMouse(mousePosition);
                if (axis != GizmoAxis.None) {
                    result = true;
                    this.StartDrag(axis);
                }
            }
            else if (this.CurrentAxis != GizmoAxis.None) {
                var entity = this.SelectionService.SelectedEntity;
                if (inputState.IsButtonHeld(MouseButton.Left)) {
                    var newPosition = this.GetPositionAlongCurrentAxis(mousePosition);
                    this.UpdatePosition(entity, newPosition);
                    this.ResetEndPoints();
                    result = true;
                }
                else {
                    this.CurrentAxis = GizmoAxis.None;
                    this.SetCursor(StandardCursorType.None);

                    var position = entity.Transform.Position;
                    var unmovedPosition = this._unmovedPosition;
                    this._undoService.Do(
                        () => this.UpdatePosition(entity, position),
                        () => this.UpdatePosition(entity, unmovedPosition));
                }
            }
            else {
                var axis = this.GetAxisUnderMouse(mousePosition);
                this.SetCursor(axis == GizmoAxis.None ? StandardCursorType.None : StandardCursorType.Hand);
            }

            return result;
        }

        /// <inheritdoc />
        protected override bool ShouldBeEnabled() {
            return this.SelectionService.SelectedEntity != null && base.ShouldBeEnabled();
        }

        private Vector2 GetPositionAlongCurrentAxis(Vector2 mousePosition) {
            // TODO: handle snapped positions when holding ctrl.
            var newPosition = mousePosition;
            if (this.CurrentAxis == GizmoAxis.X) {
                newPosition = this.MoveAlongAxis(this.NeutralAxisPosition, this.XAxisPosition, mousePosition) - (this.XAxisPosition - this.NeutralAxisPosition);
            }
            else if (this.CurrentAxis == GizmoAxis.Y) {
                newPosition = this.MoveAlongAxis(this.NeutralAxisPosition, this.YAxisPosition, mousePosition) - (this.YAxisPosition - this.NeutralAxisPosition);
            }

            return newPosition;
        }

        private void StartDrag(GizmoAxis axis) {
            this._unmovedPosition = this.NeutralAxisPosition;
            this.CurrentAxis = axis;

            if (this.CurrentAxis == GizmoAxis.X) {
                this.SetCursor(StandardCursorType.SizeWestEast);
            }
            else if (this.CurrentAxis == GizmoAxis.Y) {
                this.SetCursor(StandardCursorType.SizeNorthSouth);
            }
            else {
                this.SetCursor(StandardCursorType.DragMove);
            }
        }

        private void UpdatePosition(IGameEntity entity, Vector2 newPosition) {
            entity?.SetWorldPosition(newPosition);
        }
    }
}