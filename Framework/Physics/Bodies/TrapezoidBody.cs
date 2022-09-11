namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A body with four colliders that comprises a trapezoid.
/// </summary>
public class TrapezoidBody : QuadBody {
    private readonly LineCollider _bottomCollider = new();
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly LineCollider _leftCollider = new();
    private readonly LineCollider _rightCollider = new();
    private readonly LineCollider _topCollider = new();
    private float _bottomWidth = 2f;
    private float _height = 1f;
    private float _topWidth = 1f;

    /// <summary>
    /// Initializes a new instance of <see cref="TrapezoidBody" />.
    /// </summary>
    public TrapezoidBody() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TrapezoidBody" />.
    /// </summary>
    /// <param name="topSideWidth">The width of the top side.</param>
    /// <param name="bottomSideWidth">The width of the bottom side.</param>
    /// <param name="height">The height.</param>
    public TrapezoidBody(float topSideWidth, float bottomSideWidth, float height) : this() {
        this._topWidth = topSideWidth;
        this._bottomWidth = bottomSideWidth;
        this._height = height;
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets or sets the bottom width. Setting this is fairly expensive and should be avoided during
    /// runtime if possible.
    /// </summary>
    [DataMember]
    public float BottomWidth {
        get => this._bottomWidth;
        set {
            if (value > 0 && Math.Abs(value - this.BottomWidth) > Defaults.FloatComparisonTolerance) {
                this._bottomWidth = value;
                this.ResetColliders();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the height. Setting this is fairly expensive and should be avoided during
    /// runtime if possible.
    /// </summary>
    [DataMember]
    public float Height {
        get => this._height;
        set {
            if (value > 0 && Math.Abs(value - this.Height) > Defaults.FloatComparisonTolerance) {
                this._height = value;
                this.ResetColliders();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the top width. Setting this is fairly expensive and should be avoided during
    /// runtime if possible.
    /// </summary>
    [DataMember]
    public float TopWidth {
        get => this._topWidth;
        set {
            if (value > 0 && Math.Abs(value - this.TopWidth) > Defaults.FloatComparisonTolerance) {
                this._topWidth = value;
                this.ResetColliders();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() {
        var colliders = new List<Collider>();

        if (!this.OverrideLayersLeftEdge.IsEnabled || this.OverrideLayersLeftEdge.Value != Layers.None) {
            colliders.Add(this._leftCollider);
        }

        if (!this.OverrideLayersTopEdge.IsEnabled || this.OverrideLayersTopEdge.Value != Layers.None) {
            colliders.Add(this._topCollider);
        }

        if (!this.OverrideLayersRightEdge.IsEnabled || this.OverrideLayersRightEdge.Value != Layers.None) {
            colliders.Add(this._rightCollider);
        }

        if (!this.OverrideLayersBottomEdge.IsEnabled || this.OverrideLayersBottomEdge.Value != Layers.None) {
            colliders.Add(this._bottomCollider);
        }

        return colliders;
    }

    /// <inheritdoc />
    protected override void ResetColliders() {
        if (this._topWidth > 0f && this._bottomWidth > 0f && this._height > 0f) {
            if (this._bottomWidth > this._topWidth) {
                var sideWidth = 0.5f * (this._bottomWidth - this._topWidth);

                this._leftCollider.Start = Vector2.Zero;
                this._leftCollider.End = new Vector2(sideWidth, this._height);

                this._topCollider.Start = this._leftCollider.End;
                this._topCollider.End = new Vector2(this._topWidth + sideWidth, this._height);

                this._rightCollider.Start = this._topCollider.End;
                this._rightCollider.End = new Vector2(this._bottomWidth, 0f);

                this._bottomCollider.Start = Vector2.Zero;
                this._bottomCollider.End = this._rightCollider.End;
            }
            else {
                var sideWidth = 0.5f * (this._topWidth - this._bottomWidth);

                this._leftCollider.Start = new Vector2(0f, this._height);
                this._leftCollider.End = new Vector2(sideWidth, 0f);

                this._topCollider.Start = this._leftCollider.Start;
                this._topCollider.End = new Vector2(this._topWidth, this._height);

                this._rightCollider.Start = new Vector2(this._bottomWidth + sideWidth, 0f);
                this._rightCollider.End = this._topCollider.End;

                this._bottomCollider.Start = this._leftCollider.End;
                this._bottomCollider.End = this._rightCollider.Start;
            }

            this._leftCollider.Layers = this.OverrideLayersLeftEdge.IsEnabled ? this.OverrideLayersLeftEdge.Value : this.Layers;
            this._topCollider.Layers = this.OverrideLayersTopEdge.IsEnabled ? this.OverrideLayersTopEdge.Value : this.Layers;
            this._rightCollider.Layers = this.OverrideLayersRightEdge.IsEnabled ? this.OverrideLayersRightEdge.Value : this.Layers;
            this._bottomCollider.Layers = this.OverrideLayersBottomEdge.IsEnabled ? this.OverrideLayersBottomEdge.Value : this.Layers;

            this._leftCollider.Initialize(this);
            this._topCollider.Initialize(this);
            this._rightCollider.Initialize(this);
            this._bottomCollider.Initialize(this);
            this._boundingArea.Reset();
        }
    }

    private BoundingArea CreateBoundingArea() {
        return BoundingArea.Combine(
            this._leftCollider.BoundingArea,
            this._topCollider.BoundingArea,
            this._rightCollider.BoundingArea,
            this._bottomCollider.BoundingArea);
    }
}