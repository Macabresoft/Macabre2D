namespace Macabre2D.UI.Common;

using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using Macabresoft.AvaloniaEx;
using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MouseButton = Macabre2D.Common.MouseButton;

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
        var snapToGrid = inputState.CurrentKeyboardState.IsKeyDown(Keys.LeftControl) || inputState.CurrentKeyboardState.IsKeyDown(Keys.RightControl);

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
                    var newPosition = this.GetPositionAlongCurrentAxis(mousePosition, snapToGrid);
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

        this.TryNudgeEntity(inputState, snapToGrid);

        return result;
    }

    /// <inheritdoc />
    protected override bool ShouldBeEnabled() => this.EntityService.Selected != null && base.ShouldBeEnabled();

    private Vector2 GetPositionAlongCurrentAxis(Vector2 mousePosition, bool snapToGrid) {
        var newPosition = this.CurrentAxis switch {
            GizmoAxis.X => mousePosition - (this.XAxisPosition - this.NeutralAxisPosition),
            GizmoAxis.Y => mousePosition - (this.YAxisPosition - this.NeutralAxisPosition),
            _ => mousePosition
        };

        if (snapToGrid && this.TryGetGridContainer(out var gridContainer)) {
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

    private bool TryGetGridContainer([NotNullWhen(true)] out IGridContainer gridContainer) {
        gridContainer = null;
        return this.EntityService.Selected?.TryGetAncestor(out gridContainer) == true;
    }

    private void TryNudgeEntity(InputState inputState, bool snapToGrid) {
        if (this.EntityService.Selected is { } selected) {
            var position = selected.WorldPosition;
            var nudgeAmount = Vector2.Zero;

            if (inputState.IsKeyNewlyPressed(Keys.Left)) {
                nudgeAmount = new Vector2(-1f, nudgeAmount.Y);
            }

            if (inputState.IsKeyNewlyPressed(Keys.Up)) {
                nudgeAmount = new Vector2(nudgeAmount.X, 1f);
            }

            if (inputState.IsKeyNewlyPressed(Keys.Right)) {
                nudgeAmount = new Vector2(nudgeAmount.X + 1f, nudgeAmount.Y);
            }

            if (inputState.IsKeyNewlyPressed(Keys.Down)) {
                nudgeAmount = new Vector2(nudgeAmount.X, nudgeAmount.Y - 1f);
            }

            if (nudgeAmount != Vector2.Zero) {
                if (snapToGrid && this.TryGetGridContainer(out var gridContainer)) {
                    nudgeAmount = new Vector2(nudgeAmount.X * gridContainer.TileSize.X, nudgeAmount.Y * gridContainer.TileSize.Y);
                    position += nudgeAmount;

                    if (nudgeAmount.X != 0f) {
                        if (nudgeAmount.Y != 0f) {
                            position = gridContainer.GetNearestTilePosition(position);
                        }
                        else {
                            var gridPosition = gridContainer.GetNearestTilePosition(position);
                            position = new Vector2(gridPosition.X, position.Y);
                        }
                    }
                    else if (nudgeAmount.Y != 0f) {
                        var gridPosition = gridContainer.GetNearestTilePosition(position);
                        position = new Vector2(position.X, gridPosition.Y);
                    }
                }
                else {
                    nudgeAmount *= this.Project.UnitsPerPixel;
                    position += nudgeAmount;
                }
                
                var unmovedPosition = selected.WorldPosition;
                this._undoService.Do(
                    () => { UpdatePosition(selected, position); },
                    () => { UpdatePosition(selected, unmovedPosition); });
            }
        }
    }

    private static void UpdatePosition(ITransformable entity, Vector2 newPosition) {
        entity?.SetWorldPosition(newPosition);
    }
}