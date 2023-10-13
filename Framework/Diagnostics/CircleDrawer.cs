namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Draws a circle.
/// </summary>
[Display(Name = "Circle Drawer (Diagnostics)")]
public sealed class CircleDrawer : BaseDrawer {
    private int _complexity;
    private float _radius;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => new(-this.Radius, this.Radius);

    /// <summary>
    /// Gets or sets the complexity. This value determines the smoothness of the circle's edges.
    /// A larger value will look better, but perform worse.
    /// </summary>
    /// <remarks>
    /// Complexity is code for 'number of edges'. In reality, we can't make a perfect circle
    /// with pixels or polygons, so this is us faking it as usual. This value must be at least 3.
    /// </remarks>
    /// <value>The complexity.</value>
    [DataMember(Order = 3)]
    public int Complexity {
        get => this._complexity;

        set {
            if (value < 3) {
                value = 3;
            }

            this._complexity = value;
        }
    }

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    /// <value>The radius.</value>
    public float Radius {
        get => this._radius;
        set {
            this._radius = value;
            this.BoundingAreaChanged.SafeInvoke(this);
        }
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.Radius > 0f && this.PrimitiveDrawer != null && this.SpriteBatch is { } spriteBatch) {
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            this.PrimitiveDrawer.DrawCircle(
                spriteBatch,
                this.Project.PixelsPerUnit,
                this.Radius,
                this.WorldPosition,
                this.Complexity,
                colorOverride,
                lineThickness);
        }
    }
}