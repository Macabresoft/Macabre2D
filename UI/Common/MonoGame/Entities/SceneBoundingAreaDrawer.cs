namespace Macabre2D.UI.Common;

using System;
using System.Linq;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Draws the current scene's bounding area.
/// </summary>
public class SceneBoundingAreaDrawer : BaseDrawer {
    private readonly IEditorService _editorService;
    private readonly ISceneService _sceneService;

    /// <inheritdoc />
    public override event EventHandler BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBoundingAreaDrawer" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="sceneService">The scene service.</param>
    public SceneBoundingAreaDrawer(IEditorService editorService, ISceneService sceneService) : base() {
        this._editorService = editorService;
        this._sceneService = sceneService;

        this.UseDynamicLineThickness = true;
        this.LineThickness = 2f;
        this.Color = this._editorService.SceneBoundsColor;
        this.RenderOrder = int.MaxValue - 2;
        this.RenderOutOfBounds = true;
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => BoundingArea.Empty;

    /// <inheritdoc />
    public override RenderPriority RenderPriority { get; set; } = RenderPriority.Final;

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this._editorService.ShowSceneBounds && this.SpriteBatch is { } spriteBatch && this.PrimitiveDrawer is { } primitiveDrawer) {
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var shadowOffset = lineThickness * this.Project.UnitsPerPixel * 0.5f;
            var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

            this.DrawBoundingArea(this._sceneService.CurrentScene.BoundingArea, spriteBatch, primitiveDrawer, shadowOffsetVector, lineThickness);
        }
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        this.Render(frameTime, viewBoundingArea);
    }

    private void DrawBoundingArea(
        BoundingArea boundingArea,
        SpriteBatch spriteBatch,
        PrimitiveDrawer primitiveDrawer,
        Vector2 shadowOffsetVector,
        float lineThickness) {
        if (!boundingArea.IsEmpty) {
            var minimum = boundingArea.Minimum;
            var maximum = boundingArea.Maximum;

            var points = new[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };
            var shadowPoints = points.Select(x => x + shadowOffsetVector).ToArray();

            primitiveDrawer.DrawPolygon(spriteBatch, this.Project.PixelsPerUnit, this._editorService.DropShadowColor, lineThickness, true, shadowPoints);
            primitiveDrawer.DrawPolygon(spriteBatch, this.Project.PixelsPerUnit, this._editorService.SceneBoundsColor, lineThickness, true, points);
        }
    }
}