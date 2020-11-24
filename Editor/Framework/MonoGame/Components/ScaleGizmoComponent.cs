namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Avalonia.Input;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MouseButton = Macabresoft.Macabre2D.Framework.MouseButton;

    /// <summary>
    /// A gizmo/component that allows the user to scale entities in the editor.
    /// </summary>
    public sealed class ScaleGizmoComponent : BaseAxisGizmoComponent {
        private readonly IUndoService _undoService;
        private float _defaultLineLength;
        private Sprite _squareSprite;
        private Vector2 _unmovedScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleGizmoComponent" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="selectionService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        public ScaleGizmoComponent(
            IEditorService editorService,
            IEntitySelectionService selectionService,
            IUndoService undoService) : base(editorService, selectionService) {
            this._undoService = undoService;
        }

        /// <inheritdoc />
        public override GizmoKind GizmoKind => GizmoKind.Scale;

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            if (this.Entity.Scene.Game.GraphicsDevice is GraphicsDevice graphicsDevice) {
                this._squareSprite = PrimitiveDrawer.CreateQuadSprite(graphicsDevice, new Point(GizmoPointSize));
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

                spriteBatch.Draw(this._squareSprite, this.XAxisPosition - new Vector2(offset) + shadowOffsetVector, scale, this.EditorService.DropShadowColor);
                spriteBatch.Draw(this._squareSprite, this.YAxisPosition - new Vector2(offset) + shadowOffsetVector, scale, this.EditorService.DropShadowColor);

                base.Render(frameTime, viewBoundingArea);

                spriteBatch.Draw(this._squareSprite, this.XAxisPosition - new Vector2(offset), scale, this.EditorService.XAxisColor);
                spriteBatch.Draw(this._squareSprite, this.YAxisPosition - new Vector2(offset), scale, this.EditorService.YAxisColor);
            }
        }

        /// <inheritdoc />
        public override bool Update(FrameTime frameTime, InputState inputState) {
            var result = base.Update(frameTime, inputState);
            var mousePosition = this.Camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);

            if (inputState.IsButtonNewlyPressed(MouseButton.Left)) {
                var axis = this.GetAxisUnderMouse(mousePosition);
                if (axis == GizmoAxis.X || axis == GizmoAxis.Y) {
                    result = true;
                    this.StartDrag(axis);
                }
            }
            else if (this.CurrentAxis != GizmoAxis.None) {
                var entity = this.SelectionService.SelectedEntity;
                if (inputState.IsButtonHeld(MouseButton.Left)) {
                    var newPosition = this.GetPositionAlongCurrentAxis(mousePosition);
                    var newLineLength = Vector2.Distance(newPosition, this.NeutralAxisPosition) + this._defaultLineLength;
                    var multiplier = this.GetScaleSign(newPosition) * newLineLength / this._defaultLineLength;
                    // TODO: handle holding shift down to scale both sides at once.
                    var newScale = this.CurrentAxis == GizmoAxis.X ? 
                        new Vector2(this._unmovedScale.X * multiplier, this._unmovedScale.Y) : 
                        new Vector2(this._unmovedScale.X, this._unmovedScale.Y * multiplier);

                    if (this.CurrentAxis == GizmoAxis.X) {
                        this.XAxisPosition = newPosition;
                    }
                    else if (this.CurrentAxis == GizmoAxis.Y) {
                        this.YAxisPosition = newPosition;
                    }
                    
                    this.UpdateScale(entity, newScale);
                    result = true;
                }
                else {
                    this.CurrentAxis = GizmoAxis.None;
                    this.SetCursor(StandardCursorType.None);

                    var scale = entity.Transform.Scale;
                    var unmovedScale = this._unmovedScale;
                    this._undoService.Do(
                        () => this.UpdateScale(entity, scale),
                        () => this.UpdateScale(entity, unmovedScale));
                }
            }
            else {
                var axis = this.GetAxisUnderMouse(mousePosition);
                this.SetCursor(axis == GizmoAxis.None || axis == GizmoAxis.Neutral ? StandardCursorType.None : StandardCursorType.Hand);
            }

            return result;
        }

        /// <inheritdoc />
        protected override bool ShouldBeEnabled() {
            return this.SelectionService.SelectedEntity != null && base.ShouldBeEnabled();
        }

        private float GetScaleSign(Vector2 dragPosition) {
            var dragStartPoint = this.NeutralAxisPosition + (this.CurrentAxis == GizmoAxis.X ? new Vector2(this._defaultLineLength, 0f) : new Vector2(0f, this._defaultLineLength));
            var dragDistanceFromComponent = Vector2.Distance(dragPosition, this.NeutralAxisPosition);
            var totalDragDistance = Vector2.Distance(dragPosition, dragStartPoint);
            return totalDragDistance > this._defaultLineLength && dragDistanceFromComponent < totalDragDistance ? -1f : 1f;
        }

        private void StartDrag(GizmoAxis axis) {
            this._defaultLineLength = this.GetAxisLength();
            this._unmovedScale = this.SelectionService.SelectedEntity.Transform.Scale;
            this.CurrentAxis = axis;

            if (this.CurrentAxis == GizmoAxis.X) {
                this.SetCursor(StandardCursorType.SizeWestEast);
            }
            else if (this.CurrentAxis == GizmoAxis.Y) {
                this.SetCursor(StandardCursorType.SizeNorthSouth);
            }
        }

        private void UpdateScale(IGameEntity entity, Vector2 newScale) {
            entity.SetWorldScale(newScale);
        }
    }
}