namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Input;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MouseButton = Macabresoft.Macabre2D.Framework.MouseButton;

/// <summary>
/// A gizmo/component that allows the user to translate entities in the editor.
/// </summary>
public sealed class TranslationGizmo : BaseAxisGizmo {
    private readonly IUndoService _undoService;
    private Texture2D _neutralAxisTriangleSprite;
    private Vector2 _unmovedPosition;
    private Texture2D _xAxisArrowSprite;
    private Texture2D _yAxisArrowSprite;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationGizmo" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="entityService">The selection service.</param>
    /// <param name="undoService">The undo service.</param>
    public TranslationGizmo(
        IEditorService editorService,
        IEntityService entityService,
        IUndoService undoService) : base(editorService, entityService) {
        this._undoService = undoService;
    }

    /// <inheritdoc />
    public override GizmoKind GizmoKind => GizmoKind.Translation;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);
        
        if (this.Game.GraphicsDevice is { } graphicsDevice) {
            this._xAxisArrowSprite = PrimitiveDrawer.CreateForwardArrowSprite(graphicsDevice, GizmoPointSize);
            this._yAxisArrowSprite = PrimitiveDrawer.CreateUpwardsArrowSprite(graphicsDevice, GizmoPointSize);
            this._neutralAxisTriangleSprite = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(graphicsDevice, new Point(GizmoPointSize));
        }
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.SpriteBatch is { } spriteBatch) {
            var settings = this.Project;
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var shadowOffset = lineThickness * settings.UnitsPerPixel * 0.5f;
            var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

            var viewRatio = settings.GetPixelAgnosticRatio(viewBoundingArea.Height, this.Game.ViewportSize.Y);
            var scale = new Vector2(viewRatio);
            var offset = viewRatio * GizmoPointSize * settings.UnitsPerPixel * 0.5f; // The extra 0.5f is to center it

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._neutralAxisTriangleSprite,
                this.NeutralAxisPosition - new Vector2(offset) + shadowOffsetVector,
                scale,
                this.EditorService.DropShadowColor);

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._xAxisArrowSprite,
                this.XAxisPosition - new Vector2(offset) + shadowOffsetVector,
                scale,
                this.EditorService.DropShadowColor);

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._yAxisArrowSprite,
                this.YAxisPosition - new Vector2(offset) + shadowOffsetVector,
                scale,
                this.EditorService.DropShadowColor);

            base.Render(frameTime, viewBoundingArea);

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._xAxisArrowSprite,
                this.XAxisPosition - new Vector2(offset),
                scale,
                this.EditorService.XAxisColor);

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._yAxisArrowSprite,
                this.YAxisPosition - new Vector2(offset),
                scale,
                this.EditorService.YAxisColor);

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._neutralAxisTriangleSprite,
                this.NeutralAxisPosition + new Vector2(offset),
                -scale,
                this.EditorService.XAxisColor);

            spriteBatch.Draw(
                settings.PixelsPerUnit,
                this._neutralAxisTriangleSprite,
                this.NeutralAxisPosition - new Vector2(offset),
                scale,
                this.EditorService.YAxisColor);
        }
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        this.Render(frameTime, viewBoundingArea);
    }

    /// <inheritdoc />
    public override bool Update(FrameTime frameTime, InputState inputState) {
        var result = base.Update(frameTime, inputState);
        var mousePosition = this.Camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);

        if (inputState.IsMouseButtonNewlyPressed(MouseButton.Left)) {
            var axis = this.GetAxisUnderMouse(mousePosition);
            if (axis != GizmoAxis.None) {
                result = true;
                this.StartDrag(axis);
            }
        }
        else if (this.CurrentAxis != GizmoAxis.None) {
            if (this.EntityService.Selected is { } entity) {
                if (inputState.IsMouseButtonHeld(MouseButton.Left)) {
                    var snapToAxis = inputState.CurrentKeyboardState.IsKeyDown(Keys.LeftControl) || inputState.CurrentKeyboardState.IsKeyDown(Keys.RightControl);
                    var newPosition = this.GetPositionAlongCurrentAxis(mousePosition, snapToAxis);
                    UpdatePosition(entity, newPosition);
                    this.ResetEndPoints();
                    result = true;
                }
                else {
                    this.CurrentAxis = GizmoAxis.None;
                    this.SetCursor(StandardCursorType.None);

                    var position = entity.WorldPosition;
                    var unmovedPosition = this._unmovedPosition;
                    this._undoService.Do(
                        () => { UpdatePosition(entity, position); },
                        () => { UpdatePosition(entity, unmovedPosition); });
                    result = true;
                }
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
        return this.EntityService.Selected != null && base.ShouldBeEnabled();
    }

    private Vector2 GetPositionAlongCurrentAxis(Vector2 mousePosition, bool snapToAxis) {
        var newPosition = this.CurrentAxis switch {
            GizmoAxis.X => mousePosition - (this.XAxisPosition - this.NeutralAxisPosition),
            GizmoAxis.Y => mousePosition - (this.YAxisPosition - this.NeutralAxisPosition),
            _ => mousePosition
        };

        if (snapToAxis &&
            this.EntityService.Selected != null &&
            this.EntityService.Selected.TryGetAncestor<IGridContainer>(out var gridContainer) &&
            gridContainer != null) {
            newPosition = gridContainer.GetNearestTilePosition(newPosition);
        }

        newPosition = this.CurrentAxis switch {
            GizmoAxis.X => this.MoveAlongAxis(this.NeutralAxisPosition, this.XAxisPosition, newPosition),
            GizmoAxis.Y => this.MoveAlongAxis(this.NeutralAxisPosition, this.YAxisPosition, newPosition),
            _ => newPosition
        };

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

    private static void UpdatePosition(ITransformable entity, Vector2 newPosition) {
        entity?.SetWorldPosition(newPosition);
    }
}