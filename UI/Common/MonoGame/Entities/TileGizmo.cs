namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A gizmo for editing <see cref="ITileableEntity" />.
/// </summary>
public class TileGizmo : BaseDrawer, IGizmo {
    private readonly HashSet<Point> _addedTiles = new();
    private readonly IEditorService _editorService;
    private readonly IEntityService _entityService;
    private readonly HashSet<Point> _removedTiles = new();
    private readonly IUndoService _undoService;
    private ICamera _camera;
    private MouseButton? _currentButton;

    /// <inheritdoc />
    public override event EventHandler BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectorGizmo" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="entityService">The selection service.</param>
    /// <param name="undoService">The undo service.</param>
    public TileGizmo(IEditorService editorService, IEntityService entityService, IUndoService undoService) {
        this.UseDynamicLineThickness = true;
        this.LineThickness = 2f;

        this._editorService = editorService;
        this._entityService = entityService;
        this._undoService = undoService;

        this.Color = this._editorService.SelectionColor;
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._entityService.Selected is ITileableEntity tileable ? tileable.BoundingArea : BoundingArea.Empty;

    /// <inheritdoc />
    public GizmoKind GizmoKind => GizmoKind.Tile;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        if (!this.TryGetParentEntity(out this._camera)) {
            throw new NotSupportedException("Could not find a camera ancestor.");
        }
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.SpriteBatch is { } spriteBatch &&
            this.PrimitiveDrawer is { } drawer &&
            this.LineThickness > 0f &&
            this.Color.A > 0 &&
            this._editorService.ShowActiveTiles &&
            !this.BoundingArea.IsEmpty &&
            this._entityService.Selected is ITileableEntity tileable &&
            viewBoundingArea.Overlaps(this.BoundingArea)) {
            var settings = this.Settings;
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var shadowOffset = lineThickness * settings.UnitsPerPixel * 0.5f;
            var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

            var tileBoundingAreas = tileable.ActiveTiles
                .Select(tileable.GetTileBoundingArea)
                .Where(boundingArea => boundingArea.Overlaps(viewBoundingArea));

            foreach (var boundingArea in tileBoundingAreas) {
                var bottomLeft = boundingArea.Minimum;
                var topLeft = new Vector2(boundingArea.Minimum.X, boundingArea.Maximum.Y);
                var topRight = boundingArea.Maximum;
                var bottomRight = new Vector2(boundingArea.Maximum.X, boundingArea.Minimum.Y);

                var shadows = new[] {
                    bottomLeft + shadowOffsetVector,
                    topLeft + shadowOffsetVector,
                    topRight + shadowOffsetVector,
                    bottomRight + shadowOffsetVector,
                    bottomLeft + shadowOffsetVector,
                    topRight + shadowOffsetVector
                };

                this.PrimitiveDrawer.DrawLineStrip(spriteBatch, settings.PixelsPerUnit, this._editorService.DropShadowColor, lineThickness, shadows);

                var lineStrip = new[] {
                    bottomLeft,
                    topLeft,
                    topRight,
                    bottomRight,
                    bottomLeft,
                    topRight
                };

                this.PrimitiveDrawer.DrawLineStrip(spriteBatch, settings.PixelsPerUnit, this._editorService.SelectionColor, lineThickness, lineStrip);
            }
        }
    }

    /// <inheritdoc />
    public bool Update(FrameTime frameTime, InputState inputState) {
        if (this._camera != null && this._entityService.Selected is ITileableEntity tileable) {
            switch (this._currentButton) {
                case null when inputState.IsMouseButtonNewlyPressed(MouseButton.Left):
                    this.AddTile(tileable, inputState);
                    break;
                case null: {
                    if (inputState.IsMouseButtonNewlyPressed(MouseButton.Right)) {
                        this.RemoveTile(tileable, inputState);
                    }

                    break;
                }
                case MouseButton.Left when inputState.IsMouseButtonHeld(MouseButton.Left):
                    this.AddTile(tileable, inputState);
                    break;
                case MouseButton.Left:
                    this.CommitAdd(tileable);
                    break;
                case MouseButton.Right when inputState.IsMouseButtonHeld(MouseButton.Right):
                    this.RemoveTile(tileable, inputState);
                    break;
                case MouseButton.Right:
                    this.CommitRemove(tileable);
                    break;
            }
        }

        return true;
    }

    private void AddTile(ITileableEntity tileable, InputState inputState) {
        this._currentButton = MouseButton.Left;
        var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);
        var tile = tileable.GetTileThatContains(mousePosition);
        if (tileable.AddTile(tile)) {
            this._addedTiles.Add(tile);
        }
    }

    private void CommitAdd(ITileableEntity tileable) {
        this._currentButton = null;

        if (tileable != null && this._addedTiles.Any()) {
            var tiles = this._addedTiles.ToList();
            this._undoService.Do(() =>
            {
                foreach (var tile in tiles) {
                    tileable.AddTile(tile);
                }
            }, () =>
            {
                foreach (var tile in tiles) {
                    tileable.RemoveTile(tile);
                }
            });
        }

        this._addedTiles.Clear();
    }

    private void CommitRemove(ITileableEntity tileable) {
        this._currentButton = null;

        if (tileable != null && this._removedTiles.Any()) {
            var tiles = this._removedTiles.ToList();
            this._undoService.Do(() =>
            {
                foreach (var tile in tiles) {
                    tileable.RemoveTile(tile);
                }
            }, () =>
            {
                foreach (var tile in tiles) {
                    tileable.AddTile(tile);
                }
            });
        }

        this._removedTiles.Clear();
    }

    private void RemoveTile(ITileableEntity tileable, InputState inputState) {
        this._currentButton = MouseButton.Right;
        var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);
        var tile = tileable.GetTileThatContains(mousePosition);
        if (tileable.RemoveTile(tile)) {
            this._removedTiles.Add(tile);
        }
    }
}