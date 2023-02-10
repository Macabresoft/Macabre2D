namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Draws a line.
/// </summary>
[Display(Name = "Line Drawer (Diagnostics)")]
public sealed class LineDrawer : BaseDrawer {
    private Vector2 _endPoint;
    private Vector2 _startPoint;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => new(Vector2.Min(this.StartPoint, this.EndPoint), Vector2.Max(this.StartPoint, this.EndPoint));

    /// <summary>
    /// Gets or sets the end point.
    /// </summary>
    /// <value>The end point.</value>
    [DataMember(Order = 4)]
    public Vector2 EndPoint {
        get => this._endPoint;
        set {
            if (this.Set(ref this._endPoint, value) && this.IsInitialized) {
                this.BoundingAreaChanged.SafeInvoke(this);
            }
        }
    }

    /// <summary>
    /// Gets or sets the start point.
    /// </summary>
    /// <value>The start point.</value>
    [DataMember(Order = 3)]
    public Vector2 StartPoint {
        get => this._startPoint;
        set {
            if (this.Set(ref this._startPoint, value) && this.IsInitialized) {
                this.BoundingAreaChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.PrimitiveDrawer != null && this.StartPoint != this.EndPoint && this.SpriteBatch is { } spriteBatch) {
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            this.PrimitiveDrawer.DrawLine(spriteBatch, this.Settings.PixelsPerUnit, this.StartPoint, this.EndPoint, this.Color, lineThickness);
        }
    }
}