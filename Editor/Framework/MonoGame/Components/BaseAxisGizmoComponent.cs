namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using System;
    using System.ComponentModel;
    using Avalonia.Input;
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using IGameComponent = Microsoft.Xna.Framework.IGameComponent;

    /// <summary>
    /// A base class for gizmos that can operate on one axis or the other.
    /// </summary>
    public abstract class BaseAxisGizmoComponent : BaseDrawerComponent, IGizmo {
        /// <summary>
        /// Represents the axis a gizmo is being operated on.
        /// </summary>
        public enum GizmoAxis {
            X,
            Y,
            Neutral,
            None
        }

        /// <summary>
        /// The size used on a gizmo's point (the place where the gizmo can be grabbed by the mouse).
        /// </summary>
        protected const int GizmoPointSize = 16;

        private const float FloatingPointTolerance = 0.0001f;

        private ICameraComponent _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAxisGizmoComponent" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="selectionService">The selection service.</param>
        protected BaseAxisGizmoComponent(IEditorService editorService, IEntitySelectionService selectionService) {
            this.UseDynamicLineThickness = true;
            this.LineThickness = 1f;
            this.EditorService = editorService;
            this.EditorService.PropertyChanged += this.EditorService_PropertyChanged;

            this.SelectionService = selectionService;
            this.SelectionService.PropertyChanged += this.SelectionService_PropertyChanged;
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._camera?.BoundingArea ?? BoundingArea.Empty;

        /// <inheritdoc />
        public abstract GizmoKind GizmoKind { get; }

        /// <summary>
        /// Gets the camera.
        /// </summary>
        protected ICameraComponent Camera => this._camera;

        /// <summary>
        /// The editor service.
        /// </summary>
        protected IEditorService EditorService { get; }

        /// <summary>
        /// The selection service.
        /// </summary>
        protected IEntitySelectionService SelectionService { get; }

        /// <summary>
        /// Gets or sets the current axis being operated on.
        /// </summary>
        protected GizmoAxis CurrentAxis { get; set; } = GizmoAxis.None;

        /// <summary>
        /// Gets or sets the neutral axis position, which is the intersection of the X and Y axis.
        /// </summary>
        protected Vector2 NeutralAxisPosition { get; set; }

        /// <summary>
        /// Gets or sets the end point of the x axis line.
        /// </summary>
        protected Vector2 XAxisPosition { get; set; }

        /// <summary>
        /// Gets or sets the end point of the y axis line.
        /// </summary>
        protected Vector2 YAxisPosition { get; set; }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (this.Entity.Parent.TryGetComponent(out this._camera)) {
                this.Camera.PropertyChanged += this.Camera_PropertyChanged;
            }
            else {
                throw new NullReferenceException(nameof(this._camera));
            }

            this.ResetIsEnabled();
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer != null && this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var shadowOffset = lineThickness * GameSettings.Instance.InversePixelsPerUnit;
                var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);
                this.PrimitiveDrawer.DrawLine(spriteBatch, this.NeutralAxisPosition + shadowOffsetVector, this.XAxisPosition + shadowOffsetVector, this.EditorService.DropShadowColor, lineThickness);
                this.PrimitiveDrawer.DrawLine(spriteBatch, this.NeutralAxisPosition + shadowOffsetVector, this.YAxisPosition + shadowOffsetVector, this.EditorService.DropShadowColor, lineThickness);
                this.PrimitiveDrawer.DrawLine(spriteBatch, this.NeutralAxisPosition, this.XAxisPosition, this.EditorService.XAxisColor, lineThickness);
                this.PrimitiveDrawer.DrawLine(spriteBatch, this.NeutralAxisPosition, this.YAxisPosition, this.EditorService.YAxisColor, lineThickness);
            }
        }

        /// <inheritdoc />
        public virtual bool Update(FrameTime frameTime, InputState inputState) {
            if (this.SelectionService.SelectedEntity != null && this.CurrentAxis == GizmoAxis.None) {
                this.ResetEndPoints();
            }

            return false;
        }

        /// <summary>
        /// Gets the length of an axis line based on the view height.
        /// </summary>
        /// <returns>The length of an axis line.</returns>
        protected float GetAxisLength() {
            return this._camera.BoundingArea.Height * 0.1f;
        }

        /// <summary>
        /// Gets the axis that is currently under the mouse.
        /// </summary>
        /// <param name="mousePosition">The mouse position.</param>
        /// <returns>The axis currently under the mouse, or none if the mouse is not over an axis.</returns>
        protected GizmoAxis GetAxisUnderMouse(Vector2 mousePosition) {
            var result = GizmoAxis.None;

            var viewRatio = GameSettings.Instance.GetPixelAgnosticRatio(this.Camera.ViewHeight, this.Entity.Scene.Game.ViewportSize.Y);
            var radius = viewRatio * GizmoPointSize * GameSettings.Instance.InversePixelsPerUnit * 0.5f;
            if (Vector2.Distance(this.XAxisPosition, mousePosition) < radius) {
                result = GizmoAxis.X;
            }
            else if (Vector2.Distance(this.YAxisPosition, mousePosition) < radius) {
                result = GizmoAxis.Y;
            }
            else if (Vector2.Distance(this.NeutralAxisPosition, mousePosition) < radius) {
                result = GizmoAxis.Neutral;
            }

            return result;
        }
        
        /// <summary>
        /// Moves the end positions of the gizmo along the axis appropriately. Basically, this makes sure the drag operation is all
        /// good.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="moveToPosition">The position to move to (typically the mouse position).</param>
        /// <returns>The new position the dragged end position should be.</returns>
        protected Vector2 MoveAlongAxis(Vector2 start, Vector2 end, Vector2 moveToPosition) {
            var slope = Math.Abs(end.X - start.X) > FloatingPointTolerance ? (end.Y - start.Y) / (end.X - start.X) : 1f;
            var yIntercept = end.Y - slope * end.X;
            Vector2 newPosition;
            if (Math.Abs(slope) <= 0.5f) {
                if (Math.Abs(slope) < FloatingPointTolerance) {
                    newPosition = new Vector2(moveToPosition.X, end.Y);
                }
                else {
                    var newX = (moveToPosition.Y - yIntercept) / slope;
                    newPosition = new Vector2(newX, moveToPosition.Y);
                }
            }
            else {
                if (Math.Abs(Math.Abs(slope) - 1f) < FloatingPointTolerance) {
                    newPosition = new Vector2(end.X, moveToPosition.Y);
                }
                else {
                    var newY = slope * moveToPosition.X + yIntercept;
                    newPosition = new Vector2(moveToPosition.X, newY);
                }
            }

            return newPosition;
        }

        /// <summary>
        /// Resets the end points for each axis of the gizmo.
        /// </summary>
        /// <param name="transformable">The transformable this gizmo is attached to.</param>
        protected void ResetEndPoints() {
            var transformable = this.SelectionService.SelectedEntity;
            if (transformable != null) {
                var axisLength = this.GetAxisLength();
                var worldTransform = transformable.Transform;
                this.NeutralAxisPosition = worldTransform.Position;
                this.XAxisPosition = worldTransform.Position + new Vector2(axisLength, 0f);
                this.YAxisPosition = worldTransform.Position + new Vector2(0f, axisLength);
            }
        }

        /// <summary>
        /// Sets the cursor type for the Avalonia window.
        /// </summary>
        /// <param name="cursorType">The cursor type.</param>
        protected void SetCursor(StandardCursorType cursorType) {
            if (this.Entity.Scene.Game is IAvaloniaGame game) {
                game.CursorType = cursorType;
            }
        }

        /// <summary>
        /// A check that gets called when the selected gizmo, selected entity, or selected component changes.
        /// </summary>
        /// <returns>A value indicating whether or not this should be enabled.</returns>
        protected virtual bool ShouldBeEnabled() {
            return this.GizmoKind == this.EditorService.SelectedGizmo && !(this.SelectionService.SelectedEntity is IGameScene);
        }

        private void Camera_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ICameraComponent.ViewHeight)) {
                this.ResetEndPoints();
            }
        }

        private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IEditorService.SelectedGizmo)) {
                this.ResetIsEnabled();
            }
            else if (e.PropertyName == nameof(IEditorService.XAxisColor)) {
            }
            else if (e.PropertyName == nameof(IEditorService.YAxisColor)) {
            }
        }

        private void ResetIsEnabled() {
            this.IsEnabled = this.ShouldBeEnabled();
            this.IsVisible = this.IsEnabled;

            if (this.IsVisible) {
                this.ResetEndPoints();
            }
        }

        private void SelectionService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IEntitySelectionService.SelectedEntity) || e.PropertyName == nameof(IEntitySelectionService.SelectedComponent)) {
                this.ResetIsEnabled();
            }
        }
    }
}