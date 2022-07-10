namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A component which displays the currently selected <see cref="IEntity" />.
/// </summary>
public class SelectionDisplay : BaseDrawer {
    private readonly List<IBoundable> _boundables = new();
    private readonly IEditorService _editorService;
    private readonly IEntityService _entityService;
    private BoundingArea _boundingArea = BoundingArea.Empty;
    private IBoundable _selectedBoundable;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionDisplay" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="entityService">The selection service.</param>
    public SelectionDisplay(IEditorService editorService, IEntityService entityService) : base() {
        this.UseDynamicLineThickness = true;
        this.LineThickness = 2f;
        this._editorService = editorService;
        this._entityService = entityService;
        this._entityService.PropertyChanged += this.EntityService_PropertyChanged;

        this.Color = this._editorService.SelectionColor;
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea;

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.PrimitiveDrawer == null || this.LineThickness <= 0f || this._editorService.SelectionColor.A == 0) {
            return;
        }

        if (this.SpriteBatch is { } spriteBatch && this.PrimitiveDrawer is { } drawer && this._entityService.Selected is { } selected) {
            var settings = this.Settings;
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var shadowOffset = lineThickness * settings.UnitsPerPixel * 0.5f;
            var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

            foreach (var boundingArea in this._boundables.Select(x => x.BoundingArea)) {
                this.DrawBoundingArea(spriteBatch, drawer, settings.PixelsPerUnit, boundingArea, Color.Transparent, shadowOffsetVector, lineThickness);
            }

            if (this._selectedBoundable is { BoundingArea: { IsEmpty: false } selectedBoundingArea }) {
                this.DrawBoundingArea(spriteBatch, drawer, settings.PixelsPerUnit, selectedBoundingArea, this._editorService.SelectionColor, shadowOffsetVector, lineThickness);
            }

            if (this._editorService.SelectedGizmo == GizmoKind.Selector) {
                var (x, y) = selected.Transform.Position;

                var crosshairLength = viewBoundingArea.Height * 0.01f;
                var left = new Vector2(x - crosshairLength, y);
                var right = new Vector2(x + crosshairLength, y);
                var top = new Vector2(x, y + crosshairLength);
                var bottom = new Vector2(x, y - crosshairLength);

                drawer.DrawLine(
                    spriteBatch,
                    settings.PixelsPerUnit,
                    left + shadowOffsetVector,
                    right + shadowOffsetVector,
                    this._editorService.DropShadowColor,
                    lineThickness);

                drawer.DrawLine(
                    spriteBatch,
                    settings.PixelsPerUnit,
                    top + shadowOffsetVector,
                    bottom + shadowOffsetVector,
                    this._editorService.DropShadowColor,
                    lineThickness);

                drawer.DrawLine(spriteBatch, settings.PixelsPerUnit, left, right, this._editorService.SelectionColor, lineThickness);
                drawer.DrawLine(spriteBatch, settings.PixelsPerUnit, top, bottom, this._editorService.SelectionColor, lineThickness);
            }

            if (selected is IPhysicsBody body) {
                var colliders = body.GetColliders();
                foreach (var collider in colliders) {
                    drawer.DrawCollider(
                        collider,
                        spriteBatch,
                        settings.PixelsPerUnit,
                        this._editorService.DropShadowColor,
                        lineThickness,
                        shadowOffsetVector);

                    drawer.DrawCollider(
                        collider,
                        spriteBatch,
                        settings.PixelsPerUnit,
                        this._editorService.ColliderColor,
                        lineThickness,
                        Vector2.Zero);
                }
            }
        }
    }

    private void DrawBoundingArea(
        SpriteBatch spriteBatch,
        PrimitiveDrawer drawer,
        ushort pixelsPerUnit,
        BoundingArea boundingArea,
        Color color,
        Vector2 shadowOffsetVector,
        float lineThickness) {
        var minimum = boundingArea.Minimum;
        var maximum = boundingArea.Maximum;

        var points = new[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };

        var shadowPoints = points.Select(x => x + shadowOffsetVector).ToArray();

        drawer.DrawPolygon(spriteBatch, pixelsPerUnit, this._editorService.DropShadowColor, lineThickness, true, shadowPoints);
        drawer.DrawPolygon(spriteBatch, pixelsPerUnit, color, lineThickness, true, points);
    }

    private void EntityService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEntityService.Selected)) {
            this._boundables.Clear();

            if (!IsNullOrEmpty(this._entityService.Selected, out var selected)) {
                this._selectedBoundable = selected as IBoundable;
                this._boundables.AddRange(selected.GetDescendents<IBoundable>());

                if (this._selectedBoundable != null) {
                    this._boundables.Add(this._selectedBoundable);
                    this._boundingArea = BoundingArea.Combine(this._boundables.Select(x => x.BoundingArea).ToArray());
                    this._boundables.Remove(this._selectedBoundable);
                }
                else {
                    this._boundingArea = BoundingArea.Combine(this._boundables.Select(x => x.BoundingArea).ToArray());
                }
            }
            else {
                this._boundingArea = BoundingArea.Empty;
            }
        }
    }
}