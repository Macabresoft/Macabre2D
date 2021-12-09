namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia.Input;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MouseButton = Macabresoft.Macabre2D.Framework.MouseButton;

/// <summary>
/// A gizmo for rotating <see cref="IRotatable" /> entities in the scene editor.
/// </summary>
public class RotationGizmo : BaseAxisGizmo {
    private readonly IUndoService _undoService;
    private ICamera _camera;
    private Vector2 _knobLocation;
    private float _originalRotation;
    private Texture2D _sprite;

    /// <summary>
    /// Initializes a new instance of the <see cref="RotationGizmo" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="entityService">The selection service.</param>
    /// <param name="undoService">The undo service.</param>
    public RotationGizmo(
        IEditorService editorService,
        IEntityService entityService,
        ISceneService sceneService,
        IUndoService undoService) : base(editorService, sceneService, entityService) {
        this._undoService = undoService;
    }

    /// <inheritdoc />
    public override GizmoKind GizmoKind => GizmoKind.Rotation;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        if (!this.TryGetParentEntity(out this._camera)) {
            throw new NotSupportedException("Could not find a camera ancestor.");
        }

        if (this.Scene.Game.GraphicsDevice is GraphicsDevice graphicsDevice) {
            this._sprite = PrimitiveDrawer.CreateCircleSprite(graphicsDevice, GizmoPointSize);
        }
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this.PrimitiveDrawer is PrimitiveDrawer drawer) {
            var settings = this.Scene.Game.Project.Settings;
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var shadowOffset = lineThickness * settings.InversePixelsPerUnit;
            var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

            var viewRatio = settings.GetPixelAgnosticRatio(viewBoundingArea.Height, this.Scene.Game.ViewportSize.Y);
            var scale = new Vector2(viewRatio);
            var offset = viewRatio * GizmoPointSize * settings.InversePixelsPerUnit * 0.5f; // The extra 0.5f is to center it
            var axisLength = this.GetAxisLength();

            drawer.DrawCircle(
                spriteBatch,
                settings.PixelsPerUnit,
                axisLength,
                this.NeutralAxisPosition + shadowOffsetVector,
                32,
                this.EditorService.DropShadowColor,
                lineThickness);

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._sprite,
                this._knobLocation - new Vector2(offset) + shadowOffsetVector,
                scale,
                0f,
                this.EditorService.DropShadowColor);

            drawer.DrawCircle(
                spriteBatch,
                settings.PixelsPerUnit,
                axisLength,
                this.NeutralAxisPosition,
                64,
                this.EditorService.XAxisColor,
                lineThickness);

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._sprite,
                this._knobLocation - new Vector2(offset),
                scale,
                0f,
                this.EditorService.XAxisColor);
        }
    }

    /// <inheritdoc />
    public override bool Update(FrameTime frameTime, InputState inputState) {
        var result = base.Update(frameTime, inputState);

        if (this.EntityService.Selected is IRotatable rotatable) {
            var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);

            if (inputState.IsButtonHeld(MouseButton.Left)) {
                if (this.CurrentAxis != GizmoAxis.None) {
                    this.UpdateDrag(rotatable, mousePosition);
                    result = true;
                }
                else {
                    result = this.TryStartDrag(rotatable, mousePosition);
                }
            }
            else if (this.CurrentAxis != GizmoAxis.None) {
                this.TryEndDrag(rotatable);
                result = true;
            }
            else {
                var axis = this.GetAxisUnderMouse(mousePosition);
                this.SetCursor(axis == GizmoAxis.None ? StandardCursorType.None : StandardCursorType.Hand);
                this._knobLocation = this.XAxisPosition;
            }
        }

        return result;
    }

    private Vector2 GetKnobLocation(Vector2 entityPosition, Vector2 mousePosition) {
        return entityPosition + (mousePosition - entityPosition).GetNormalized() * this.GetAxisLength();
    }

    private static float GetNewAngle(ITransformable transformable, Vector2 mousePosition) {
        var worldPosition = transformable.Transform.Position;
        var (x, y) = mousePosition - worldPosition;
        return (float)Math.Atan2(y, x);
    }

    private void TryEndDrag(IRotatable rotatable) {
        if (this.CurrentAxis != GizmoAxis.None && Math.Abs(rotatable.Rotation - this._originalRotation) > float.Epsilon) {
            var originalRotation = this._originalRotation;
            var newRotation = rotatable.Rotation;

            this._undoService.Do(() => { rotatable.Rotation = newRotation; }, () => { rotatable.Rotation = originalRotation; });
        }

        this.CurrentAxis = GizmoAxis.None;
        this._knobLocation = this.XAxisPosition;
    }

    private bool TryStartDrag(IRotatable rotatable, Vector2 mousePosition) {
        var result = false;

        if (this.GetAxisUnderMouse(mousePosition) == GizmoAxis.X) {
            this._originalRotation = rotatable.Rotation;
            this.CurrentAxis = GizmoAxis.X;
            result = true;
            this.SetCursor(StandardCursorType.DragMove);
        }

        return result;
    }

    private void UpdateDrag(IRotatable rotatable, Vector2 mousePosition) {
        rotatable.Rotation = GetNewAngle(rotatable, mousePosition);
        this._knobLocation = this.GetKnobLocation(rotatable.Transform.Position, mousePosition);
    }
}