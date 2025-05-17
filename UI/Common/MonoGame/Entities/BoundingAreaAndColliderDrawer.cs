namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Draws every bounding area and collider in the scene.
/// </summary>
public class BoundingAreaAndColliderDrawer : BaseDrawer {
    private readonly IEditorService _editorService;
    private readonly ISceneService _sceneService;

    /// <inheritdoc />
    public override event EventHandler BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingAreaAndColliderDrawer" /> class.
    /// </summary>
    /// <param name="editorService">The editor service.</param>
    /// <param name="sceneService">The scene service.</param>
    public BoundingAreaAndColliderDrawer(IEditorService editorService, ISceneService sceneService) : base() {
        this._editorService = editorService;
        this._sceneService = sceneService;

        this.UseDynamicLineThickness = true;
        this.LineThickness = 2f;
        this.Color = this._editorService.SelectionColor;
        this.RenderOrder = int.MaxValue - 2;
        this.RenderOutOfBounds = true;
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => BoundingArea.Empty;

    /// <inheritdoc />
    public override RenderPriority RenderPriority { get; set; } = RenderPriority.Final;

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this._editorService.ShowBoundingAreasAndColliders && this.SpriteBatch is { } spriteBatch && this.PrimitiveDrawer is { } primitiveDrawer) {
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var shadowOffset = lineThickness * this.Project.UnitsPerPixel * 0.5f;
            var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

            var boundingAreas = this._sceneService.CurrentScene.GetDescendants<IBoundable>().Select(x => x.BoundingArea);
            foreach (var boundingArea in boundingAreas) {
                this.DrawBoundingArea(boundingArea, spriteBatch, primitiveDrawer, shadowOffsetVector, lineThickness);
            }

            var colliders = this._sceneService.CurrentScene.GetDescendants<ISimplePhysicsBody>().Select(x => x.Collider);
            foreach (var collider in colliders) {
                this.DrawCollider(collider, spriteBatch, primitiveDrawer, shadowOffsetVector, lineThickness);
            }
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
            primitiveDrawer.DrawPolygon(spriteBatch, this.Project.PixelsPerUnit, this._editorService.SelectionColor, lineThickness, true, points);
        }
    }

    private void DrawCollider(
        Collider collider,
        SpriteBatch spriteBatch,
        PrimitiveDrawer primitiveDrawer,
        Vector2 shadowOffsetVector,
        float lineThickness) {
        primitiveDrawer.DrawCollider(
            collider,
            spriteBatch,
            this.Project.PixelsPerUnit,
            this._editorService.DropShadowColor,
            lineThickness,
            shadowOffsetVector);

        primitiveDrawer.DrawCollider(
            collider,
            spriteBatch,
            this.Project.PixelsPerUnit,
            this._editorService.ColliderColor,
            lineThickness,
            Vector2.Zero);
    }
}