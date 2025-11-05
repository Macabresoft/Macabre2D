namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.ComponentModel;
using System.Linq;
using Avalonia.Input;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// A base class for gizmos that can operate on one axis or the other.
/// </summary>
public abstract class BaseAxisGizmo : BaseDrawer, IGizmo {
    /// <summary>
    /// The size used on a gizmo's point (the place where the gizmo can be grabbed by the mouse).
    /// </summary>
    protected const int GizmoPointSize = 16;

    private const float FloatingPointTolerance = 0.0001f;
    private ICamera _camera;

    /// <inheritdoc />
    public override event EventHandler BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAxisGizmo" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="entityService">The selection service.</param>
    protected BaseAxisGizmo(IEditorService editorService, IEntityService entityService) {
        this.UseDynamicLineThickness = true;
        this.LineThickness = 1f;
        this.EditorService = editorService;
        this.EditorService.PropertyChanged += this.EditorService_PropertyChanged;

        this.EntityService = entityService;
        this.EntityService.PropertyChanged += this.SelectionService_PropertyChanged;
        this.RenderOrder = int.MaxValue;
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._camera?.BoundingArea ?? BoundingArea.Empty;

    /// <inheritdoc />
    public abstract GizmoKind GizmoKind { get; }

    /// <summary>
    /// Gets the camera.
    /// </summary>
    protected ICamera Camera => this._camera;

    /// <summary>
    /// Gets or sets the current axis being operated on.
    /// </summary>
    protected GizmoAxis CurrentAxis { get; set; } = GizmoAxis.None;

    /// <summary>
    /// Gets the editor service.
    /// </summary>
    protected IEditorService EditorService { get; }

    /// <summary>
    /// Gets the selection service.
    /// </summary>
    protected IEntityService EntityService { get; }

    /// <summary>
    /// Gets or sets the neutral axis position, which is the intersection of the X and Y axis.
    /// </summary>
    protected Vector2 NeutralAxisPosition { get; private set; }

    /// <summary>
    /// Gets or sets the end point of the x axis line.
    /// </summary>
    protected Vector2 XAxisPosition { get; private set; }

    /// <summary>
    /// Gets or sets the end point of the y axis line.
    /// </summary>
    protected Vector2 YAxisPosition { get; private set; }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);
        this.CurrentAxis = GizmoAxis.None;
        this.RenderPriority = Enum.GetValues<RenderPriority>().Max();
        this.RenderOrder = int.MaxValue;
        this.LineThickness = 3f;

        if (this.TryGetAncestor(out this._camera)) {
            this.Camera.BoundingAreaChanged += this.Camera_BoundingAreaChanged;
            this.Camera.PropertyChanged += this.Camera_PropertyChanged;
        }
        else {
            throw new NullReferenceException(nameof(this._camera));
        }

        this.ResetIsEnabled();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.SpriteBatch is { } spriteBatch && this.PrimitiveDrawer is { } drawer) {
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var lineOffset = lineThickness * this.Project.UnitsPerPixel * -0.5f;
            var lineOffsetVector = new Vector2(-lineOffset, lineOffset);
            var pixelsPerUnit = this.Project.PixelsPerUnit;

            drawer.DrawLine(spriteBatch, pixelsPerUnit, this.NeutralAxisPosition, this.XAxisPosition, this.EditorService.DropShadowColor, lineThickness);
            drawer.DrawLine(spriteBatch, pixelsPerUnit, this.NeutralAxisPosition, this.YAxisPosition, this.EditorService.DropShadowColor, lineThickness);
            drawer.DrawLine(spriteBatch, pixelsPerUnit, this.NeutralAxisPosition + lineOffsetVector, this.XAxisPosition + lineOffsetVector, this.EditorService.XAxisColor, lineThickness);
            drawer.DrawLine(spriteBatch, pixelsPerUnit, this.NeutralAxisPosition + lineOffsetVector, this.YAxisPosition + lineOffsetVector, this.EditorService.YAxisColor, lineThickness);
        }
    }

    /// <inheritdoc />
    public virtual bool Update(FrameTime frameTime, InputState inputState) {
        if (this.EntityService.Selected != null && this.CurrentAxis == GizmoAxis.None) {
            this.ResetEndPoints();
        }

        return false;
    }

    /// <summary>
    /// Gets the axis that is currently under the mouse.
    /// </summary>
    /// <param name="mousePosition">The mouse position.</param>
    /// <returns>The axis currently under the mouse, or none if the mouse is not over an axis.</returns>
    protected GizmoAxis GetAxisUnderMouse(Vector2 mousePosition) {
        var result = GizmoAxis.None;

        var viewRatio = this.Project.GetPixelAgnosticRatio(this.Camera.ActualViewHeight, this.Game.ViewportSize.Y);
        var radius = viewRatio * GizmoPointSize * this.Project.UnitsPerPixel * 0.5f;
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
    protected void ResetEndPoints() {
        var transformable = this.EntityService.Selected;
        if (transformable != null) {
            var axisLength = this.GetAxisLength();
            var worldTransform = transformable.WorldPosition;
            this.NeutralAxisPosition = worldTransform;
            this.XAxisPosition = worldTransform + new Vector2(axisLength, 0f);
            this.YAxisPosition = worldTransform + new Vector2(0f, axisLength);
        }
    }

    /// <summary>
    /// Sets the cursor type for the Avalonia window.
    /// </summary>
    /// <param name="cursorType">The cursor type.</param>
    protected void SetCursor(StandardCursorType cursorType) {
        this.EditorService.CursorType = cursorType;
    }

    /// <summary>
    /// A check that gets called when the selected gizmo, selected entity, or selected component changes.
    /// </summary>
    /// <returns>A value indicating whether this should be enabled.</returns>
    protected virtual bool ShouldBeEnabled() => this.GizmoKind == this.EditorService.SelectedGizmo && this.EntityService.Selected is not IScene;

    private void Camera_BoundingAreaChanged(object sender, EventArgs e) {
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void Camera_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ICamera.ActualViewHeight)) {
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

    private float GetAxisLength() {
        var result = 0.1f;
        if (this._camera != null) {
            result *= this._camera.ActualViewHeight;
        }

        return result;
    }

    private void ResetIsEnabled() {
        this.IsEnabled = this.ShouldBeEnabled();
        this.ShouldRender = this.IsEnabled;

        if (this.ShouldRender) {
            this.ResetEndPoints();
        }
    }

    private void SelectionService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEntityService.Selected)) {
            this.ResetIsEnabled();
        }
    }

    /// <summary>
    /// Represents the axis a gizmo is being operated on.
    /// </summary>
    protected enum GizmoAxis {
        X,
        Y,
        Neutral,
        None
    }
}