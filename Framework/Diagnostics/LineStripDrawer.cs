namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Draws a line strip.
/// </summary>
[Display(Name = "Line Strip Drawer (Diagnostics)")]
public sealed class LineStripDrawer : BaseDrawer {
    private readonly List<Vector2> _vertices = new();
    private BoundingArea _boundingArea;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineStripDrawer" /> class.
    /// </summary>
    public LineStripDrawer() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineStripDrawer" /> class.
    /// </summary>
    /// <param name="points">The points.</param>
    public LineStripDrawer(IEnumerable<Vector2> points) {
        this.ResetPoints(points);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea;

    /// <summary>
    /// Gets the points to draw. These are represented in world coordinates.
    /// </summary>
    /// <value>The points.</value>
    public IReadOnlyCollection<Vector2> Vertices => this._vertices;

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.PrimitiveDrawer != null && this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this._vertices.Any()) {
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            this.PrimitiveDrawer.DrawLineStrip(spriteBatch, this.Scene.Game.Project.Settings.PixelsPerUnit, this.Color, lineThickness, this.Vertices.ToArray());
        }
    }

    /// <summary>
    /// Clears the collection of points and replaces it with the newly provided points.
    /// </summary>
    /// <param name="points">The points.</param>
    public void ResetPoints(IEnumerable<Vector2> points) {
        this._vertices.Clear();
        this._vertices.AddRange(points);
        this._boundingArea = this.CreateBoundingArea();
    }

    private BoundingArea CreateBoundingArea() {
        return new BoundingArea(
            this.Vertices.Min(p => p.X),
            this.Vertices.Max(p => p.X),
            this.Vertices.Min(p => p.Y),
            this.Vertices.Max(p => p.Y));
    }
}